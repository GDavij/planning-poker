using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.AddStory;

public record AddStoryCommand(string Name, string? StoryNumber);

public record AddStoryCommandResponse(long StoryId);

public class AddStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;
    private readonly INotificationService _notificationService;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;
    
    public AddStoryCommandHandler(IApplicationDbContext dbContext, ListStoriesQueryHandler listStoriesQueryHandler, INotificationService notificationService, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _listStoriesQueryHandler = listStoriesQueryHandler;
        _notificationService = notificationService;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<AddStoryCommandResponse?> Handle(long matchId, AddStoryCommand command)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories)
                                            .FirstAsync(m => m.MatchId == matchId);

        var createdStory = match.RegisterNewStoryAs(command.Name, command.StoryNumber, _notificationService);

        if (_notificationService.HasNotifications())
        {
            return null;
        }

        await _dbContext.SaveChangesAsync(CancellationToken.None);


        var stories = await _listStoriesQueryHandler.Handle(match.MatchId, CancellationToken.None);
        await _matchSignalRIntegrationIntegrationClient.NotifyCurrentListOfStoriesForMatchAsync(match, stories);

        return new AddStoryCommandResponse(createdStory!.StoryId);
    }
}