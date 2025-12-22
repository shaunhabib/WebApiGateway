using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    public IActionResult CreateJob([FromBody] JobRequest request)
    {
        try
        {
            var jobData = System.Text.Json.JsonSerializer.Serialize(request);
            _jobService.SendToRpc("https://peptransport.ebsbd.com:50053", "Create", jobData);
            
            return Ok(new { 
                jobId = request.JobId, 
                status = "submitted",
                message = jobData
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}