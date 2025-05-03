using System.Net;
using Application.Abstractions.SignalR;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.SelectStory;


public record SelectStoryCommandResponse(long StoryId);

public class SelectStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;

    public SelectStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<SelectStoryCommandResponse?> Handle(long matchId, long storyId)
    {
        if (await _dbContext.Stories.FirstAsync(s => s.StoryId == storyId &&
                                                           s.MatchId == matchId) is Story story)
        {
            await _matchSignalRIntegrationIntegrationClient.NotifySelectStoryToVoteForMatchAsync(story);

            await foreach (var storyPoint in _dbContext.StoryPoints.Include(sp => sp.Participant)
                                                                                 .Where(sp => sp.StoryId == storyId).AsAsyncEnumerable())
            {
                if (storyPoint.Participant.SignalRConnectionId is null)
                {
                    continue;
                }

                await _matchSignalRIntegrationIntegrationClient.NotifyStoryVoteAsync(storyPoint);
            }
            return new SelectStoryCommandResponse(story.StoryId);
        }
        
        _notificationService.AddNotification("Could not find Story to Select to.", "Story.NotFound", HttpStatusCode.NotFound);
        return null;
    }
}