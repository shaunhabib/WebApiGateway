using ApiGateway.Abstraction;
using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class GatewayService : Gateway
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

    public override void StartListening()
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
        ProcessJobCreation(args.Payload);
    }

    public override void ProcessJobCreation(string payload)
    {
        Console.WriteLine($"Processing job creation: {payload}");
        SendToRpc("JobService", "ProcessJob", payload);
    }

    private void ProcessNotificationSuccess(string payload)
    {
        Console.WriteLine($"Processing notification success: {payload}");
    }
    
}