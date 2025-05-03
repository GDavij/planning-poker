using Application.Abstractions.SignalR;
using Application.SignalR;
using Application.UseCases.Management.Accounts.CreateAccount;
using Application.UseCases.Management.Accounts.Me;
using Application.UseCases.Planning.Matches.ApproveJoinRequest;
using Application.UseCases.Planning.Matches.CloseMatch;
using Application.UseCases.Planning.Matches.CreateMatch;
using Application.UseCases.Planning.Matches.ListMatches;
using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Matches.ListRoles;
using Application.UseCases.Planning.Stories.AddStory;
using Application.UseCases.Planning.Stories.DeleteStory;
using Application.UseCases.Planning.Stories.ListStories;
using Application.UseCases.Planning.Stories.SelectStory;
using Application.UseCases.Planning.Stories.UpdateStory;
using Application.UseCases.Planning.Stories.VoteStory;
using Domain.Abstractions.SignalR;
using Microsoft.Extensions.DependencyInjection;
using WebApi.UseCases.Commands.Matches.JoinMatch;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        /* Management */
        // Accounts
        services.AddScoped<CreateAccountCommandHandler>();
        services.AddScoped<GetMeQueryHandler>();
        
        /* Planning */
        // Matches
        services.AddScoped<ApproveJoinRequestCommandHandler>();
        services.AddScoped<CloseMatchCommandHandler>();
        services.AddScoped<CreateMatchCommandHandler>();
        services.AddScoped<JoinMatchCommandHandler>();
        services.AddScoped<ListMatchesQueryHandler>();
        services.AddScoped<ListParticipantsQueryHandler>();
        services.AddScoped<ListRolesQueryHandler>();
        
        // Stories
        services.AddScoped<AddStoryCommandHandler>();
        services.AddScoped<DeleteStoryCommandHandler>();
        services.AddScoped<ListStoriesQueryHandler>();
        services.AddScoped<SelectStoryCommandHandler>();
        services.AddScoped<UpdateStoryCommandHandler>();
        services.AddScoped<VoteStoryCommandHandler>();

        return services;
    }

    public static IServiceCollection AddSignalRIntegrationClients(this IServiceCollection services)
    {
        services.AddSingleton<IMatchSignalRIntegrationIntegrationClient, MatchSignalRIntegrationIntegrationClient>();

        return services;
    }
}