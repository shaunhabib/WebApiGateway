namespace ApiGateway.Abstraction;

public abstract class Gateway
{
    public abstract void ProcessJobCreation(string payload);
    public abstract void StartListening();
}