using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomController : ControllerBase
{
    private readonly CustomService _apiGateway;

    public CustomController(CustomService apiGateway)
    {
        _apiGateway = apiGateway;
    }

    [HttpPost]
    public IActionResult CreateJob([FromBody] JobRequest request)
    {
        try
        {
            var jobData = System.Text.Json.JsonSerializer.Serialize(request);
            _apiGateway.SendToRpc("http://localhost:50053", "Create", jobData);
            
            return Ok(new { 
                jobId = request.JobId, 
                status = "submitted",
                message = "Job submitted for processing"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}