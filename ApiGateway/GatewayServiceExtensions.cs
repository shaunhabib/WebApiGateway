using ApiGateway.Abstraction;
using ApiGateway.Config;
using ApiGateway.Services;
using PEPRPC;
using PEPSignal;

namespace ApiGateway;

public static class GatewayServiceExtensions
{
    public static IServiceCollection AddGatewayService(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<GatewayConfig>(configuration.GetSection("Gateway"));

        var options = configuration.GetSection("Gateway").Get<GatewayConfig>() ?? new GatewayConfig();

        services.AddSingleton(sp => new PEPGRPC());

        var signalRConfig = new PEPSignal.MessageTransceiverConfig
        {
            Host = options.SignalRHost,
            Port = options.SignalRPort
        };

        var signalRService = new PEPSignalR();
        signalRService.Configure(signalRConfig);

        foreach (var hub in options.SignalRHubs)
        {
            signalRService.RegisterHub(hub, services);
        }

        services.AddSingleton(sp => signalRService);

        // Register services
        services.AddSingleton<SignalRRouterService>();
        services.AddSingleton<JobService>(sp => sp.GetRequiredService<SignalRRouterService>().GetJobService());
        // services.AddSingleton<NotificationService>(sp => sp.GetRequiredService<SignalRRouterService>().GetNotificationService());

        return services;
    }
}