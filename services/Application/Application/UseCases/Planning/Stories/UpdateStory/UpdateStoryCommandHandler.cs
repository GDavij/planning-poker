using System.Net;
using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.UpdateStory;

public record UpdateStoryCommand(string Name, string? StoryNumber, short Order);

public record UpdateStoryCommandResponse(long StoryId);

public class UpdateStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;

    public UpdateStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, ListStoriesQueryHandler listStoriesQueryHandler, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _listStoriesQueryHandler = listStoriesQueryHandler;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<UpdateStoryCommandResponse?> Handle(long matchId, long storyId, UpdateStoryCommand command)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories)
                                            .FirstAsync(m => m.MatchId == matchId, CancellationToken.None);

        var storyToUpdate = match.GetStoryWithId(storyId);
        if (storyToUpdate is null)
        {
            _notificationService.AddNotification("Could not find Story to update.", "Story.NotFound", HttpStatusCode.NotFound);
            return null;
        }

        if (match.HasAnyStoryInOrderToUpdateThatIsNot(storyToUpdate, command.Order, out Story? storyWithSameOrder))
        {
            match.SwapOrderForStories(storyToUpdate, storyWithSameOrder!);
        }

        storyToUpdate.RenameTo(command.Name);
        storyToUpdate.TagStoryWithNumber(command.StoryNumber);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var stories = await _listStoriesQueryHandler.Handle(match.MatchId, CancellationToken.None);
        await _matchSignalRIntegrationIntegrationClient.NotifyCurrentListOfStoriesForMatchAsync(match, stories);

        return new UpdateStoryCommandResponse(storyId);
    }
}