using Domain.Abstractions;
using WebApi.Services;
using WebApi.UseCases.Commands.Accounts.CreateAccount;
using WebApi.UseCases.Commands.Matches.ApproveJoinRequest;
using WebApi.UseCases.Commands.Matches.CreateMatch;
using WebApi.UseCases.Commands.Matches.JoinMatch;
using WebApi.UseCases.Queries.Matches.ListMatches;
using WebApi.UseCases.Queries.Matches.ListRoles;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection InjectUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateAccountCommandHandler>();
        services.AddScoped<ApproveJoinRequestCommandHandler>();
        services.AddScoped<CreateMatchCommandHandler>();
        services.AddScoped<JoinMatchCommandHandler>();
        services.AddScoped<ListMatchesQueryHandler>();
        services.AddScoped<ListRolesQueryHandler>();

        // Notification Used to handle Failure notification to other layers
        services.AddScoped<INotificationService, NotificationService>();
        
        return services;
    }
}