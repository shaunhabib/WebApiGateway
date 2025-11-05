using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;
using System.Text.Json;

namespace ApiGateway.Services;

public class SignalRRouterService
{
    private readonly JobService _jobService;
    private readonly NotificationService _notificationService;
    private readonly AuthService _authService;
    private readonly PEPJobService _PEPJobService;
    private readonly RegisterService _registerService;
    private readonly PEPSignalR _signalService;
    private readonly GatewayConfig _options;
    private readonly Dictionary<string, bool> _registeredListeners;

    public SignalRRouterService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options)
    {
        _options = options.Value;
        _signalService = signalService;
        _registeredListeners = new Dictionary<string, bool>();
        
        _jobService = new JobService(signalService, rpcService, options);
        _notificationService = new NotificationService(signalService, rpcService, options);
        _authService = new AuthService(signalService, rpcService, options);
        _PEPJobService = new PEPJobService(signalService, rpcService, options);
        _registerService = new RegisterService(signalService, rpcService, options);
    }

    public void StartListening()
    {
        foreach (var listener in _options.SignalRListeners)
        {
            var listenerKey = $"{listener.Key}:{listener.Value}";
            if (!_registeredListeners.ContainsKey(listenerKey))
            {
                _signalService.ListenForData(listener.Key, listener.Value, OnRawSignalRDataReceived);
                _registeredListeners[listenerKey] = true;
            }
            else
            {
                Console.WriteLine($"Listener already registered: {listenerKey}");
            }
        }
        
        Console.WriteLine("MessageRouterService started");
    }

    private void OnRawSignalRDataReceived(string channel, string payload, PEPSignal.DataReadyEventArgs args)
    {
        var message = args.Payload;
        try
        {
            RouteToService(channel, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }

    private void RouteToService(string channel, string message)
    {
        switch (channel.ToLower())
        {
            case "job":
                _jobService.Process(message);
                break;
                
            case "notification":
                _notificationService.Process(message);
                break;

            case "auth":
                _authService.Process(message);
                break;
            case "pepjob":
                _PEPJobService.Process(message);
                break;
            case "poc":
                _registerService.Process(message);
                break;

            default:
                break;
        }
    }

    public JobService GetJobService() => _jobService;
    public NotificationService GetNotificationService() => _notificationService;
    public AuthService GetAuthService() => _authService;
    public PEPJobService GetPEPJobService() => _PEPJobService;
    public RegisterService GetRegisterService() => _registerService;
}