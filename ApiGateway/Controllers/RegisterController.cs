using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private const string _serverURL2 = "http://192.168.110.43:5280";
        private readonly RegisterService _registerService;

        public RegisterController(RegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                string rpcData = JsonSerializer.Serialize(request);
                _registerService.SendToRpc(_serverURL2, "Register", rpcData);
                return Ok("Register is processing");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
