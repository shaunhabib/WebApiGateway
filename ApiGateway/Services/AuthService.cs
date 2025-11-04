using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services
{
    public class AuthService : GatewayService
    {
        public AuthService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
        {
        }

        public override void Process(string message)
        {
            try
            {
                Console.WriteLine($"Auth Service receive data: {message}");
                SendToSignalR("Notify", "Response", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
