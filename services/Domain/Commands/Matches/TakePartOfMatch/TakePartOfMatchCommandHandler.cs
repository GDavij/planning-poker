using Domain.Abstractions;
using Domain.Abstractions.Auth;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Commands.Matches.TakePartOfMatch;

public class TakePartOfMatchCommandHandler : IRequestHandler<TakePartOfMatchCommand, TakePartOfMatchCommandResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ICurrentAccount _currentAccount;
    private readonly IAuthenticationService _authService;
    
    public TakePartOfMatchCommandHandler(
        IApplicationDbContext dbContext,
        INotificationService notificationService,
        ICurrentAccount currentAccount,
        IAuthenticationService authService)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _currentAccount = currentAccount;
        _authService = authService;
    }

    public async Task<TakePartOfMatchCommandResponse> Handle(TakePartOfMatchCommand request, CancellationToken cancellationToken)
    {
        var roleDoesNotExists = !(await _dbContext.Roles.AnyAsync(r => r.RoleId == request.RoleId, cancellationToken));
        if (roleDoesNotExists)
        {
            _notificationService.AddNotification("Could not find Selected Role", "role.doesNotExists");
            return default;
        }
        
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => m.MatchId == request.MatchId, cancellationToken);
        if (match is null)
        {
            _notificationService.AddNotification("Could not find match to participate", "match.doesNotExists");
            return default;
        }

        if (match.HasClosed)
        {
            _notificationService.AddNotification("Match is already closed, can't participate", "match.closed");
            return default;
        }

        if (match.AccountId != _currentAccount.AccountId && request.AuthGuid == Guid.Empty)
        {
            _notificationService.AddNotification("Could not participate match, missing permission", "match.noPermissionToJoin");
            return default;
        } 
        else if (request.AuthGuid != Guid.Empty)
        {
            if (!_authService.HasValidAuthCode(request.AuthGuid,
                                     string.Concat(request.AuthGuid, request.MatchId)).Result)
            {
                _notificationService.AddNotification("Could not participate match, missing permission", "match.invalidAuthCode");
                return default;
            }
        }
        
        var participant = new Participant(_currentAccount, match, request.RoleId);

        if (request.ShouldSpectate)
        {
            participant.SpectateMatch();
        }

        if (participant.Join(match, _notificationService))
        {
            await _dbContext.SaveChangesAsync(CancellationToken.None);
         
            return new TakePartOfMatchCommandResponse(participant.MatchId);
        }

        return default;
    }
}