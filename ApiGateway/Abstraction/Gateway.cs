namespace ApiGateway.Abstraction;

public abstract class Gateway
{
    // public abstract void ProcessJobCreation(string payload);
    public abstract void StartListening();
    public abstract void SendToSignalR(string channel, string action, string data);
}