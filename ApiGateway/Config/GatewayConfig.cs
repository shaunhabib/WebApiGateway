namespace ApiGateway.Config;

public class GatewayConfig
{
    public string SignalRHost { get; set; } = "localhost";
    public int SignalRPort { get; set; } = 5165;
    public List<string> SignalRHubs { get; set; } = new List<string>();
    public Dictionary<string, string> SignalRListeners { get; set; } = new Dictionary<string, string>();
}