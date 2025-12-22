using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;
using System.Diagnostics;

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
                string info = $"Auth Service received data: {message}";
                Console.WriteLine(info);


                SendToSignalR("Notify", "Response", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception: {ex.Message}");
            }
        }
    }
}
