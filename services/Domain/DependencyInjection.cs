using Domain.Abstractions;
using Domain.Abstractions.Shared;
using Domain.Services;
using Domain.Services.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DependencyInjection
{
    public static IServiceCollection InjectUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
        });

        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<IMatchAllocationService, MatchAllocationService>();
        
        return services;
    }
}