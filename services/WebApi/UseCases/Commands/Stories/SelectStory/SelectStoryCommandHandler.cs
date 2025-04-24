using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;

namespace WebApi.UseCases.Commands.Stories.SelectStory;

public class SelectStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<MatchHub> _hubContext;


    public SelectStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IHubContext<MatchHub> hubContext)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _hubContext = hubContext;
    }

    public async Task Handle(long matchId, long storyId)
    {
        if (await _dbContext.Stories.AnyAsync(s => s.StoryId == storyId &&
                                             s.MatchId == matchId))
        {
            await _hubContext.Clients.Group(matchId.ToString()).SendAsync("SelectStoryToVoteAs", storyId);
            return;
        }
        
        _notificationService.AddNotification("Could not find Story to Select to.", "Story.NotFound");
    }
}