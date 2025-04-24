using System.Reflection.Metadata;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;
using WebApi.UseCases.Queries.Stories;
using WebApi.UseCases.Queries.Stories.ListStories;

namespace WebApi.UseCases.Commands.Stories.UpdateStory;

public class UpdateStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;

    public UpdateStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IHubContext<MatchHub> hubContext, ListStoriesQueryHandler listStoriesQueryHandler)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _listStoriesQueryHandler = listStoriesQueryHandler;
    }

    public async Task Handle(long matchId, long storyId, UpdateStoryCommand command)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories)
                                            .FirstAsync(m => m.MatchId == matchId, CancellationToken.None);

        var storyToUpdate = match.GetStoryWithId(storyId);
        if (storyToUpdate is null)
        {
            _notificationService.AddNotification("Could not find Story to update.", "Story.NotFound");
            return;
        }

        if (match.HasAnyStoryInOrderToUpdateThatIsNot(storyToUpdate, command.Order, out Story? storyWithSameOrder))
        {
            match.SwapOrderForStories(storyToUpdate, storyWithSameOrder!);
        }

        storyToUpdate.RenameTo(command.Name);
        storyToUpdate.TagStoryWithNumber(command.StoryNumber);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var stories = await _listStoriesQueryHandler.Handle(match.MatchId, CancellationToken.None);
        await _hubContext.Clients.Group(match.MatchId.ToString()).SendAsync("UpdateStoriesOfMatchWith", stories);
    }
}