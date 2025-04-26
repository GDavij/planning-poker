using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;
using WebApi.UseCases.Queries.Stories;
using WebApi.UseCases.Queries.Stories.ListStories;

namespace WebApi.UseCases.Commands.Stories.AddStory;

public class AddStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly ListStoriesQueryHandler _listStoriesQueryHandler;
    private readonly INotificationService _notificationService;
    
    public AddStoryCommandHandler(IApplicationDbContext dbContext, IHubContext<MatchHub> hubContext, ListStoriesQueryHandler listStoriesQueryHandler, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _listStoriesQueryHandler = listStoriesQueryHandler;
        _notificationService = notificationService;
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
        await _hubContext.Clients.Group(match.MatchId.ToString()).SendAsync("UpdateStoriesOfMatchWith", stories);

        return new AddStoryCommandResponse(createdStory!.StoryId);
    }
}