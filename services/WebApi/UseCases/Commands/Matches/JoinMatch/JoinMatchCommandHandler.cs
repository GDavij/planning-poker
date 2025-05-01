using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;
using WebApi.UseCases.Queries.Matches.ListParticipants;

namespace WebApi.UseCases.Commands.Matches.JoinMatch;

public class JoinMatchCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;
    private readonly IHubContext<MatchHub> _matchHubContext;
    private readonly ListParticipantsQueryHandler _listParticipantsQueryHandler;
    private readonly INotificationService _notificationService;

    public JoinMatchCommandHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount, IHubContext<MatchHub> matchHubContext, ListParticipantsQueryHandler listParticipantsQueryHandler, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
        _matchHubContext = matchHubContext;
        _listParticipantsQueryHandler = listParticipantsQueryHandler;
        _notificationService = notificationService;
    }

    public async Task<JoinMatchCommandResponse?> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
    {
        var match = await _dbContext.Matches.Include(m => m.Participants.Where(p => p.AccountId == _currentAccount.AccountId))
                                            .FirstOrDefaultAsync(m => m.MatchId == request.MatchId, cancellationToken);

        if (match is null)
        {
            await _matchHubContext.Clients.Client(request.ConnectionId)
                .SendAsync("ReceiveErrorAsync", new Notification("Could not find match.", "Matches.NotFound"));
            return null;
        }
        
        var participant = match.Participants.FirstOrDefault();

        if (participant is null)
        {
            // Temporally create an auto join for player (Just to send to my teacher cause i don't have time to implement a better alternative for now...)
            
            var role = await _dbContext.Roles.FirstAsync(r => r.RoleId == 15, CancellationToken.None);
            participant = new Participant(_currentAccount, role);
            participant.Join(match, _notificationService);

            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            await Handle(request, cancellationToken);

            return new JoinMatchCommandResponse();
        }


        participant.ConnectedAt(request.ConnectionId);
        await _matchHubContext.Groups.AddToGroupAsync(request.ConnectionId, match.MatchId.ToString(), CancellationToken.None);
        
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var participants = await _listParticipantsQueryHandler.Handle(match.MatchId, CancellationToken.None);
        
        await _matchHubContext.Clients.Group(match.MatchId.ToString()).SendAsync("CurrentListOfParticipantsIs", participants, cancellationToken: cancellationToken);
        await _matchHubContext.Clients.Client(request.ConnectionId).SendAsync("ApproveJoinRequest", CancellationToken.None);
        
        return new JoinMatchCommandResponse();
    }
}