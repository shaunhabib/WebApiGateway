using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class CustomService : GatewayService
{
    public CustomService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
    {
    }

    public override void ProcessJobCreation(string payload)
    {
        Console.WriteLine("CustomService ProcessJobCreation");
    }
}