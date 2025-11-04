using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class NotificationService : GatewayService
{
    public NotificationService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) 
        : base(signalService, rpcService, options)
    {
    }

    public override void Process(string message)
    {
        Console.WriteLine($"Notification Service {message}");
    }
}