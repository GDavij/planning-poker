using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;

namespace WebApi.UseCases.Commands.Matches.CloseMatch;

public class CloseMatchCommandHandler
{
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public CloseMatchCommandHandler(IHubContext<MatchHub> hubContext, IApplicationDbContext dbContext, ICurrentAccount currentAccount, INotificationService notificationService)
    {
        _hubContext = hubContext;
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    public async Task Handle(long matchId)
    {
        var match = await _dbContext.Matches.FirstAsync(m => m.MatchId == matchId);

        if (match.HasClosed)
        {
            _notificationService.AddNotification("Match already closed", "Match.AlreadyClosed");
            return;
        }
        
        match.Close();
        await _dbContext.SaveChangesAsync();
        
        await _hubContext.Clients.Group(matchId.ToString()).SendAsync("MatchClosed");
    }
}