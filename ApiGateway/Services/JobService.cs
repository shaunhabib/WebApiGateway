using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services;

public class JobService : GatewayService
{
    public JobService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
    {
    }
    
    public override void ProcessJobCreation(string payload)
    {
        Console.WriteLine("JobService ProcessJobCreation");
    }
}