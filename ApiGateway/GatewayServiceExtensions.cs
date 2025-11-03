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
    
        services.AddSingleton(sp =>
        {
            var grpcConfig = new PEPRPC.MessageTransceiverConfig
            {
                Host = options.GrpcHost,
                Port = options.GrpcPort
            };
            return new PEPGRPC(grpcConfig);
        });

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

        services.AddSingleton<GatewayService>();
    
        return services;
    }
}