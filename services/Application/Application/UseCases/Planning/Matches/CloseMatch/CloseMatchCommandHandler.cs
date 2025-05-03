using System.Net;
using Application.Abstractions.SignalR;
using Domain.Abstractions;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Matches.CloseMatch;

public record CloseMatchCommandResponse(long MatchId);

public class CloseMatchCommandHandler
{
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public CloseMatchCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<CloseMatchCommandResponse?> Handle(long matchId)
    {
        var match = await _dbContext.Matches.FirstAsync(m => m.MatchId == matchId);

        if (match.HasClosed)
        {
            _notificationService.AddNotification("Match already closed", "Match.AlreadyClosed", HttpStatusCode.Conflict);
            return null;
        }
        
        match.Close();
        await _dbContext.SaveChangesAsync();

        await _matchSignalRIntegrationIntegrationClient.NotifyClosedMatchAsync(match);
        return new CloseMatchCommandResponse(match.MatchId);
    }
}