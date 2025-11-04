using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class GatewayService
{
    protected readonly PEPGRPC _rpcService;
    protected readonly PEPSignalR _signalService;
    protected readonly GatewayConfig _options;

    public GatewayService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options)
    {
        _signalService = signalService;
        _rpcService = rpcService;
        _options = options.Value;
    }
    
    public void SendToRpc(string channel, string action, string data)
    {
        _rpcService.SendData<string, string, string>(channel, action, data);
    }

    public virtual void Process(string message)
    {
        
    }
    public void SendToSignalR(string channel, string action, string data)
    {
        _signalService.SendData<string, string, bool>(channel, action, data);
    }
}