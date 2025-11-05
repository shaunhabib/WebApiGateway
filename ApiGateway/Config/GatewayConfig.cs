namespace ApiGateway.Config;

public class GatewayConfig
{
    public string GrpcHost { get; set; } = "localhost";
    public int GrpcPort { get; set; } = 50053;
    public string SignalRHost { get; set; } = "localhost";
    public int SignalRPort { get; set; } = 5164;
    
    public List<string> SignalRHubs { get; set; }
    public Dictionary<string, string> SignalRListeners { get; set; }
}