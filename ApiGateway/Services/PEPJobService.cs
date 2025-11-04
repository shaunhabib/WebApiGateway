using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services
{
    public class PEPJobService : GatewayService
    {
        public PEPJobService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
        {
        }

        public override void Process(string message)
        {
            Console.WriteLine($"PEP Job service {message}");
            SendToSignalR("PEPNotification", "PEPResponse", message);
        }
    }
}
