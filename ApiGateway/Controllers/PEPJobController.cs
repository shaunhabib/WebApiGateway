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
    public class PEPJobController : ControllerBase
    {
        private readonly PEPJobService _pepJobService;

        public PEPJobController(PEPJobService pepJobService)
        {
            _pepJobService = pepJobService;
        }
        [HttpPost]
        public IActionResult CreateJob([FromBody] PEPJobRequest jobData)
        {
            try
            {
                var gatewayTimestamp = DateTimeOffset.UtcNow;

                jobData.GatewayTimestamp = gatewayTimestamp;

                string jobSerialized = JsonSerializer.Serialize(jobData);
                _pepJobService.SendToRpc("http://localhost:50093", "create", jobSerialized);

                return Ok(new
                {
                    Message = "Job created successfully",
                    JobData = jobData,
                    GatewayTimestamp = gatewayTimestamp
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            
        }

    }
}
