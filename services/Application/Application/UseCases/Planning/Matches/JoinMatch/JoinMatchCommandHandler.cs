using System.Net;
using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Matches.ListParticipants;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Matches.JoinMatch;

public record JoinMatchCommand(long MatchId, string ConnectionId);

public record JoinMatchCommandResponse;

public class JoinMatchCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;
    private readonly ListParticipantsQueryHandler _listParticipantsQueryHandler;
    private readonly INotificationService _notificationService;

    public JoinMatchCommandHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient, ListParticipantsQueryHandler listParticipantsQueryHandler, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
        _listParticipantsQueryHandler = listParticipantsQueryHandler;
        _notificationService = notificationService;
    }

    public async Task<JoinMatchCommandResponse?> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
    {
        var match = await _dbContext.Matches.Include(m => m.Participants.Where(p => p.AccountId == _currentAccount.AccountId))
                                            .FirstOrDefaultAsync(m => m.MatchId == request.MatchId, cancellationToken);

        if (match is null)
        {
            _notificationService.AddNotification("Could not find match.", "Matches.NotFound", HttpStatusCode.NotFound);
            return null;
        }
        
        var participant = match.Participants.FirstOrDefault();

        if (participant is null)
        {
            // Temporally create an auto join for player (Just to send to my teacher because don't have time to implement a better alternative for now...)
            // IN FUTURE IT SHOULD REQUEST A APPROVAL FROM THE PARTICIPANTS OF THE MATCH...
            
            var role = await _dbContext.Roles.FirstAsync(r => r.RoleId == 15, CancellationToken.None);
            participant = new Participant(_currentAccount, role);
            participant.Join(match, _notificationService);

            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            await Handle(request, cancellationToken);

            return new JoinMatchCommandResponse();
        }


        participant.ConnectedAt(request.ConnectionId);
        await _matchSignalRIntegrationIntegrationClient.JoinParticipantToMatchAsync(participant, match);
        
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var currentParticipants = await _listParticipantsQueryHandler.Handle(match.MatchId, CancellationToken.None);

        await _matchSignalRIntegrationIntegrationClient.NotifyCurrentListOfParticipantsOfMatch(match, currentParticipants);
        await _matchSignalRIntegrationIntegrationClient.NotifyApproveJoinRequestForParticipantAsync(participant);
        
        return new JoinMatchCommandResponse();
    }
}