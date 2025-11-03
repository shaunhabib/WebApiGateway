using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PEPRPC;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PEPGRPC _pepRPC;
        private const string _serverURL = "http://localhost:50053";
        public AuthController()
        {
            _pepRPC = new PEPGRPC();
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var rpcRequest = new LoginRequestTorpc
                {
                    UserName = request.UserName,
                    Password = request.Password,
                    Age = request.Age,
                    SubmittedDate = DateTimeOffset.UtcNow
                };
                string rpcData = JsonSerializer.Serialize(rpcRequest);
                _pepRPC.SendData<string, string, string>(_serverURL, "Login", rpcData);
                return Ok("Login is processing");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
    }

    public class LoginRequestTorpc
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public DateTimeOffset SubmittedDate { get; set; }
    }
}
