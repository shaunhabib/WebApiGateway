using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;

namespace ApiGateway.Services
{
    public class RegisterService : GatewayService
    {
        public RegisterService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
        {
        }

        public override void Process(string message)
        {
            try
            {
                Console.WriteLine($"Register Service receive data: {message}");
                SendToSignalR("Notify", "RegisterResponse", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
