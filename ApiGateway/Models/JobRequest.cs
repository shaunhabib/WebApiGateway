using System.Text.Json.Serialization;

namespace ApiGateway.Models;

public class JobRequest
{
    [JsonPropertyName("jobId")]
    public string JobId { get; set; }  = Guid.NewGuid().ToString();
        
    [JsonPropertyName("receiverName")]
    public string ReceiverName { get; set; } = string.Empty;
        
    [JsonPropertyName("senderName")]
    public string SenderName { get; set; } = string.Empty;

    [JsonPropertyName("qty")]
    public int Qty { get; set; } = 0;
        
    [JsonPropertyName("submittedDate")]
    public DateTime SubmittedDate { get; set; }
        
    [JsonPropertyName("apiGetawayReceivedDate")]
    public DateTime ApiGetawayReceivedDate { get; set; }

    [JsonPropertyName("jobServiceReceivedDate")]
    public DateTime JobServiceReceivedDate { get; set; }

    [JsonPropertyName("jobFexReceivedDate")]
    public DateTime JobFexReceivedDate { get; set; }
}