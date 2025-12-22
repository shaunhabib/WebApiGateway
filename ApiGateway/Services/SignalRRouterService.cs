using ApiGateway.Config;
using ApiGateway.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PEPCore;
using PEPCore.Secret;
using PEPRPC;
using PEPSignal;
using System.Text.Json;

namespace ApiGateway.Services;

public class SignalRRouterService
{
    private readonly JobService _jobService;
    private readonly PEPSignalR _signalService;
    private readonly GatewayConfig _options;
    private readonly Dictionary<string, bool> _registeredListeners;
    private readonly string DbConnectionstring;
    private const string DbConnectionKey = "POC-DB-ConnectionString";
    private SqlConnection connection;

    public SignalRRouterService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options)
    {
        _options = options.Value;
        _signalService = signalService;
        _registeredListeners = new Dictionary<string, bool>();
        _jobService = new JobService(signalService, rpcService, options);

        DbConnectionstring = Secret.GetValueAsync(ScopeType.Global, null, DbConnectionKey, null)
                .GetAwaiter()
                .GetResult();
        connection = DB.Connection(DbConnectionstring);
        connection.Open();
    }

    public void StartListening()
    {
        foreach (var listener in _options.SignalRListeners)
        {
            var listenerKey = $"{listener.Key}:{listener.Value}";
            if (!_registeredListeners.ContainsKey(listenerKey))
            {
                _signalService.ListenForData(listener.Key, listener.Value, OnRawSignalRDataReceived);
                _registeredListeners[listenerKey] = true;
            }
            else
            {
                Console.WriteLine($"Listener already registered: {listenerKey}");
            }
        }

        Console.WriteLine("MessageRouterService started");
    }

    private void OnRawSignalRDataReceived(string channel, string payload, PEPSignal.DataReadyEventArgs args)
    {
        try
        {
            var res = JsonSerializer.Deserialize<LoginResponse>(payload);
            if(res is null)
                throw new Exception("Deserialized payload is null");

            if(!UpdateData(res.MessageId, res.SubmittedTimeFromFex))
                throw new Exception("Failed to update performance log.");

            if (channel != res.Channel.ToLower())
                RouteToService(channel, args.Payload);
            else
                _signalService.SendData<string, string, string>(res.SignalRClientIdentifier, args.Payload);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }

    private bool UpdateData(int messageId, TimeSpan SubmittedTimeFromFex)
    {
        string updateSql = @"
                    UPDATE PerformanceLog
                    SET DeltaC = ABS(DATEDIFF(MILLISECOND, @SubmittedTimeFromFex, @ReceivedTimeFromGW)),
                        SubmitFromFeX = @SubmitFromFeX,
                        ReceivedAtGW = @ReceivedAtGW
                    WHERE MessageId = @MessageId;";

        var param = Utils.KVPList(new[]
        {
                ("@MessageId", (object)messageId),
                ("@SubmittedTimeFromFex", (object)SubmittedTimeFromFex!),
                ("@ReceivedTimeFromGW", DateTime.UtcNow.TimeOfDay),
                ("@ReceivedAtGW", DateTime.UtcNow.TimeOfDay.ToString()),
                ("@SubmitFromFeX", SubmittedTimeFromFex.ToString()),
            });

        var rowsAffected = DB.Execute(updateSql, param, connection, false, false);
        return rowsAffected.Value > 0;
    }

    private void RouteToService(string channel, string message)
    {
        switch (channel.ToLower())
        {
            case "job":
                _jobService.Process(message);
                break;

            default:
                break;
        }
    }

    public JobService GetJobService() => _jobService;

    public class LoginResponse : MessageBase
    {
        public bool IsSuccess { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
        public TimeSpan SubmittedTimeFromFex { get; set; }
    }

}