using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class GatewayService
{
    private readonly PEPGRPC _rpcService;
    private readonly PEPSignalR _signalService;
    private readonly GatewayConfig _options;

    public GatewayService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options)
    {
        _signalService = signalService;
        _rpcService = rpcService;
        _options = options.Value;
    }

    public void StartListening()
    {
        foreach (var listener in _options.SignalRListeners)
        {
            _signalService.ListenForData(listener.Key, listener.Value, OnRawSignalRDataReceived);
        }
    }
    
    public void SendToRpc(string channel, string action, string data)
    {
        _rpcService.SendData<string, string, string>(channel, action, data);
    }
    
    private void OnRawSignalRDataReceived(string channel, string payload, PEPSignal.DataReadyEventArgs args)
    {
        Console.WriteLine($"Processing data from channel '{args.Channel}': {args.Payload}");

        try
        {
            switch (args.Channel)
            {
                case "Job" when args.Action == "Create":
                    ProcessJobCreation(args.Payload);
                    break;
                case "Notification" when args.Action == "Success":
                    ProcessNotificationSuccess(args.Payload);
                    break;
                default:
                    Console.WriteLine($"Unhandled channel/action: {args.Channel}/{args.Action}");
                    break;
            }

            _signalService.SendData<string, string, string>(channel, args.Action, args.Payload);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in raw SignalR data receiver: {ex.Message}");
        }
    }

    private void ProcessJobCreation(string payload)
    {
        Console.WriteLine($"Processing job creation: {payload}");
        SendToRpc("JobService", "ProcessJob", payload);
    }

    private void ProcessNotificationSuccess(string payload)
    {
        Console.WriteLine($"Processing notification success: {payload}");
    }
    
}