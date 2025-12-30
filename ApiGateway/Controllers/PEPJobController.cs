using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PEPCore;
using PEPCore.Secret;
using PEPCore.Settings;
using PEPRPC;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PEPJobController : ControllerBase
    {
        private readonly PEPGRPC _pepRpc;
        private readonly string DbConnectionstring;
        private const string DbConnectionKey = "POC-DB-ConnectionString";

        public PEPJobController(PEPGRPC pepRpc)
        {
            DbConnectionstring = Secret.GetValueAsync(ScopeType.Global, null, DbConnectionKey, null)
                .GetAwaiter()
                .GetResult();
            _pepRpc = pepRpc;
        }
        [HttpPost]
        public IActionResult CreateJob([FromBody] PEPJobRequest jobData)
        {
            try
            {
                var gatewayTimestamp = DateTimeOffset.UtcNow;

                jobData.GatewayTimestamp = gatewayTimestamp;

                string jobSerialized = JsonSerializer.Serialize(jobData);
                _pepRpc.SendData<string, string, string>("http://localhost:50093", "create", jobSerialized);

                return Ok(new
                {
                    Message = "Job creation is processing using this payload",
                    JobData = jobData,
                    GatewayTimestamp = gatewayTimestamp
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        public IActionResult GetCategory20()
        {
            try
            {
                var result = _pepRpc.SendData<string, string, string>("http://localhost:50093", "getCategory20", $"category20");
                if(result.IsSuccess)
                {
                    var doc = JsonSerializer.Deserialize<JsonElement>(result.Value);
                    var message = doc.GetProperty("message").GetString();
                    var resposePaylod = JsonSerializer.Deserialize<JsonElement>(message);

                    var resultArray = resposePaylod.GetProperty("value");

                    return Ok(resultArray);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCategory50()
        {
            try
            {
                var result = _pepRpc.SendData<string, string, string>("http://localhost:50093", "getCategory50", $"category50");
                if (result.IsSuccess)
                {
                    var doc = JsonSerializer.Deserialize<JsonElement>(result.Value);
                    var message = doc.GetProperty("message").GetString();
                    var resposePaylod = JsonSerializer.Deserialize<JsonElement>(message);

                    var resultArray = resposePaylod.GetProperty("value");

                    return Ok(resultArray);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCategory100([FromQuery] int count = 100)
        {
            try
            {
                var result = _pepRpc.SendData<string, string, string>("http://localhost:50093", "getCategory100", $"{count}");
                if (result.IsSuccess)
                {
                    var doc = JsonSerializer.Deserialize<JsonElement>(result.Value);
                    var message = doc.GetProperty("message").GetString();
                    var resposePaylod = JsonSerializer.Deserialize<JsonElement>(message);

                    var resultArray = resposePaylod.GetProperty("value");

                    return Ok(resultArray);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult getJobsGrpc()
        {
            try
            {
                using var _client = new HttpClient();
                _client.BaseAddress = new Uri("https://localhost:7136");

                using var connection = DB.Connection(DbConnectionstring);
                connection.Open();

                var gatewayTimestamp = DateTime.UtcNow.TimeOfDay;

                var result = _pepRpc.SendData<string, string, string>("http://localhost:50093", "Get", $"{gatewayTimestamp}");


                if (result.IsSuccess)
                {
                    var value = result.Value;
                    var doc = JsonSerializer.Deserialize<JsonElement>(value);
                    var message = doc.GetProperty("message").GetString();
                    var resposePaylod = JsonSerializer.Deserialize<JsonElement>(message);

                    var apiTimestamp = resposePaylod.GetProperty("apiTimeStamp").GetString();
                    var responseServiceTimeStamp = resposePaylod.GetProperty("responseServiceTimeStamp").GetString();
                    var rowId = resposePaylod.GetProperty("id").GetInt32();
                    var resultArray = resposePaylod.GetProperty("result").GetProperty("value");


                    var gatewayReciveTimestamp = DateTime.UtcNow.TimeOfDay;

                    string updateSql = @"
                UPDATE [POCDemo].[dbo].[get_job_request]
                SET
                    service_recive_timestamp = @responseServiceTimeStamp,
                    gateway_recive_timestamp = @gatewayReciveTimestamp,
                    delta_res_service_and_gateway = ABS(DATEDIFF(MILLISECOND, @responseServiceTimeStamp, @gatewayReciveTimestamp)),
                    delta_res_api_and_service = ABS(DATEDIFF(MILLISECOND, @responseServiceTimeStamp, @apiTimestamp)),
                    total_of_grpc = ABS(DATEDIFF(MILLISECOND, @gateway_timestamp, @gatewayReciveTimestamp))
                WHERE id = @id;
                ";



                    var param = Utils.KVPList(new (string, object)[]
                    {
                    ("@gateway_timestamp", gatewayTimestamp),
                    ("@gatewayReciveTimestamp", gatewayReciveTimestamp),
                    ("@responseServiceTimeStamp", TimeSpan.Parse(responseServiceTimeStamp)),
                    ("@apiTimestamp", TimeSpan.Parse(apiTimestamp)),
                    ("@id", rowId)
                    });

                    var rowsAffected = DB.Execute(updateSql, param, connection, false, false);


                    return Ok(new
                    {
                        resultArray
                    });
                }
                return Ok(new
                {
                    status = "submitted"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult getJobsRest()
        {
            try
            {
                using var _client = new HttpClient();
                _client.BaseAddress = new Uri("https://localhost:7136");
                using var connection = DB.Connection(DbConnectionstring);
                connection.Open();

                var gatewayTimestamp = DateTime.UtcNow.TimeOfDay;
                var query = new Dictionary<string, string?>()
                {
                    ["gatewayTimestamp"] = $"{gatewayTimestamp}"
                };
                string url = QueryHelpers.AddQueryString("/api/job/GetJobs", query);
                var response = _client.GetAsync(url).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = response.Content.ReadAsStringAsync().Result;
                    return BadRequest(new 
                    { 
                        status= response.StatusCode,
                        error = errorContent
                    });
                }
                var gatewayReciveTimestamp = DateTime.UtcNow.TimeOfDay;

                var json = response.Content.ReadAsStringAsync().Result;
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                var apiTimestamp = doc.GetProperty("apiTimeStamp").GetString();
                var rowId = doc.GetProperty("id").GetInt32();
                var resultArray = doc.GetProperty("result").GetProperty("value");


                string updateSql = @"
                UPDATE [POCDemo].[dbo].[get_job_request]
                SET
                    gateway_recive_timestamp = @gatewayReciveTimestamp,
                    delta_res_api_and_gateway = ABS(DATEDIFF(MILLISECOND, @gatewayReciveTimestamp, @apiTimestamp)),
                    total_of_rest = ABS(DATEDIFF(MILLISECOND, @gateway_timestamp, @gatewayReciveTimestamp))
                WHERE id = @id;
                ";



                var param = Utils.KVPList(new (string, object)[]
                {
                    ("@gateway_timestamp", gatewayTimestamp),
                    ("@gateway_recive_timestamp", gatewayTimestamp),
                    ("@gatewayReciveTimestamp", gatewayReciveTimestamp),
                    ("@apiTimestamp", TimeSpan.Parse(apiTimestamp)),
                    ("@id", rowId)
                });

                var rowsAffected = DB.Execute(updateSql, param, connection, false, false);


                return Ok(new
                {
                    resultArray
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
