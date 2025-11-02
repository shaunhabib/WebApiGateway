using PEPSignal;
using System.Text.Json;

namespace ApiGateway
{
    public static class SignalRHelper
    {
        private static PEPSignalR _pep;
        private const string _authHub = "Auth";
        static SignalRHelper()
        {
            _pep = new PEPSignalR("localhost", 5164);
        }

        public static void RegisterHub(IServiceCollection services)
        {
            _pep.RegisterHub("Notify", services);
            _pep.RegisterHub(_authHub, services);
        }

        public static bool SendMessage(string hubEndpoint,string action, string message)
        {
            try
            {
                var result = _pep.SendData<string, string, bool>(hubEndpoint, action, message);
                Console.WriteLine($"Message sent to '{action}': {message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message: {ex.Message}");
                return false;
            }
        }

        public static void StartListening()
        {
            try
            {
                _pep.ListenForData(_authHub, "Login", DataReceived);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to listen for data: {ex.Message}");
            }
        }

        private static void DataReceived(string channel, string data, DataReadyEventArgs args)
        {
            var res = JsonSerializer.Deserialize<LoginResponse>(args.Payload);
            Console.WriteLine($"Received from client: {data}");
            SendMessage("Notify", "Response", args.Payload);
        }

        public class LoginResponse
        {
            public bool IsSuccess { get; set; }
            public string UserName { get; set; }
            public string Token { get; set; }
        }
    }
}
