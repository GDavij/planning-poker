using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;
using WebApi.UseCases.Queries.Stories;
using WebApi.UseCases.Queries.Stories.ListStories;

namespace WebApi.UseCases.Commands.Stories.DeleteStory;

public class DeleteStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;

    public DeleteStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IHubContext<MatchHub> hubContext, ListStoriesQueryHandler listStoriesQueryHandler)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _listStoriesQueryHandler = listStoriesQueryHandler;
    }

    public async Task Handle(long matchId, long storyId)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories.OrderBy(s => s.Order))
                                            .FirstAsync(m => m.MatchId == matchId);

        var story = match.GetStoryWithId(storyId);
        if (story is null)
        {
            _notificationService.AddNotification("Could not find story to delete.", "Story.NotFound");
            return;
        }

        match.RemoveStory(story);

        await _dbContext.SaveChangesAsync(CancellationToken.None);
        
        var stories = await _listStoriesQueryHandler.Handle(match.MatchId, CancellationToken.None);
        await _hubContext.Clients.Group(match.MatchId.ToString()).SendAsync("UpdateStoriesOfMatchWith", stories);
    }
}