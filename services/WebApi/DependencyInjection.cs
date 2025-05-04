using Application.Abstractions.SignalR;
using Domain.Abstractions.SignalR;
using WebApi.SignalR;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRServiceClients(this IServiceCollection services)
    {
        services.AddSingleton<ISignalRService<IMatchSignalRIntegrationIntegrationClient>, MatchSignalRServiceClient>();

        return services;
    }
}