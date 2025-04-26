using Domain.Abstractions;
using WebApi.Services;
using WebApi.UseCases.Commands.Accounts.CreateAccount;
using WebApi.UseCases.Commands.Matches.ApproveJoinRequest;
using WebApi.UseCases.Commands.Matches.CreateMatch;
using WebApi.UseCases.Commands.Matches.JoinMatch;
using WebApi.UseCases.Commands.Stories.AddStory;
using WebApi.UseCases.Commands.Stories.DeleteStory;
using WebApi.UseCases.Commands.Stories.SelectStory;
using WebApi.UseCases.Commands.Stories.UpdateStory;
using WebApi.UseCases.Commands.Stories.VoteStory;
using WebApi.UseCases.Queries.Accounts.Me;
using WebApi.UseCases.Queries.Matches.ListMatches;
using WebApi.UseCases.Queries.Matches.ListParticipants;
using WebApi.UseCases.Queries.Matches.ListRoles;
using WebApi.UseCases.Queries.Stories;
using WebApi.UseCases.Queries.Stories.ListStories;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection InjectUseCases(this IServiceCollection services)
    {
        // Accounts
        services.AddScoped<CreateAccountCommandHandler>();
        services.AddScoped<GetMeQueryHandler>();
        
        // Matches
        services.AddScoped<ApproveJoinRequestCommandHandler>();
        services.AddScoped<CreateMatchCommandHandler>();
        services.AddScoped<JoinMatchCommandHandler>();
        services.AddScoped<ListMatchesQueryHandler>();
        
        // Stories
        services.AddScoped<AddStoryCommandHandler>();
        services.AddScoped<UpdateStoryCommandHandler>();
        services.AddScoped<DeleteStoryCommandHandler>();
        services.AddScoped<SelectStoryCommandHandler>();
        services.AddScoped<VoteStoryCommandHandler>();
        services.AddScoped<ListStoriesQueryHandler>();
        
        // Participants
        services.AddScoped<ListParticipantsQueryHandler>();
        
        // Roles
        services.AddScoped<ListRolesQueryHandler>();

        // Notification Used to handle Failure notification to other layers
        services.AddScoped<INotificationService, NotificationService>();
        
        return services;
    }
}