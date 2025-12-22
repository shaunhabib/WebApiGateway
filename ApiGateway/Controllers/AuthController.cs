using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PEPCore;
using PEPCore.Secret;
using PEPRPC;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string _serverURL = "http://localhost:50051";
        //private readonly AuthService _authService;
        protected readonly PEPGRPC _rpcService;
        private readonly string DbConnectionstring;
        private const string DbConnectionKey = "POC-DB-ConnectionString";
        public AuthController(PEPGRPC rpcService)
        {
            //_authService = authService;
            _rpcService = rpcService;

            DbConnectionstring = Secret.GetValueAsync(ScopeType.Global, null, DbConnectionKey, null)
                .GetAwaiter()
                .GetResult();
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!InsertData(request.MessageId))
                {
                    return BadRequest("Failed to insert performance log.");
                }

                request.SubmittedTimeFromGW = DateTime.UtcNow.TimeOfDay;
                string rpcData = JsonSerializer.Serialize(request);

                var res = _rpcService.SendData<string, string, string>(_serverURL, "Login", rpcData);

                if (res.IsFailure)
                    return BadRequest(res.Error);

                return Ok("Login is processing");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //[HttpPost]
        //public IActionResult Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        for (int i = 1; i <= request.NumOfReq; i++)
        //        {
        //            request.MessageId = i;
                    
        //            if (!InsertData(request.MessageId))
        //            {
        //                return BadRequest("Failed to insert performance log.");
        //            }

        //            request.SubmittedTimeFromGW = DateTime.UtcNow.TimeOfDay;
        //            string rpcData = JsonSerializer.Serialize(request);

        //            var res = _rpcService.SendData<string, string, string>(_serverURL, "Login", rpcData);
        //        }
                
        //        //if (res.IsFailure)
        //        //    return BadRequest(res.Error);

        //        return Ok("Login is processing");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
           
        //}

        private bool InsertData(int messageId)
        {
            using var connection = DB.Connection(DbConnectionstring);
            connection.Open();

            string sql = @"
                INSERT INTO PerformanceLog (MessageId, DeltaA, DeltaB, DeltaC)
                VALUES (@MessageId, @DeltaA, @DeltaB, @DeltaC);";

            var param = Utils.KVPList(new[]
            {
                ("@MessageId", (object)messageId),
                ("@DeltaA", 0),
                ("@DeltaB", 0),
                ("@DeltaC", 0),
            });

            var rowsAffected = DB.Execute(sql, param, connection, false, false);
            return rowsAffected.Value > 0;
        }
    }
}
