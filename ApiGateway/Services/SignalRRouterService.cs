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
            var res = JsonSerializer.Deserialize<MessageBase>(payload);
            if(res is null)
                throw new Exception("Deserialized payload is null");

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

}