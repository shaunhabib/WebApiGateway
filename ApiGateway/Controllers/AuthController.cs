using ApiGateway.Models;
using ApiGateway.Services;
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
        private const string _serverURL = "http://localhost:5280";
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                string rpcData = JsonSerializer.Serialize(request);
                _authService.SendToRpc(_serverURL, "Login", rpcData);
                return Ok("Login is processing");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
