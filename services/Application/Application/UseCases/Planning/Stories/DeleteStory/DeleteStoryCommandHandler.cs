using System.Net;
using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.DeleteStory;

public record DeleteStoryCommandResponse(long StoryId);

public class DeleteStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;

    public DeleteStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, ListStoriesQueryHandler listStoriesQueryHandler, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _listStoriesQueryHandler = listStoriesQueryHandler;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<DeleteStoryCommandResponse?> Handle(long matchId, long storyId)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories.OrderBy(s => s.Order))
                                            .FirstAsync(m => m.MatchId == matchId);

        var story = match.GetStoryWithId(storyId);
        if (story is null)
        {
            _notificationService.AddNotification("Could not find story to delete.", "Story.NotFound", HttpStatusCode.NotFound);
            return null;
        }

        match.RemoveStory(story);

        await _dbContext.SaveChangesAsync(CancellationToken.None);
        
        var stories = await _listStoriesQueryHandler.Handle(match.MatchId, CancellationToken.None);
        await _matchSignalRIntegrationIntegrationClient.NotifyCurrentListOfStoriesForMatchAsync(match, stories);

        return new DeleteStoryCommandResponse(story.StoryId);
    }
}