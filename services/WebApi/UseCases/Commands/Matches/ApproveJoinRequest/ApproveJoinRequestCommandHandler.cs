using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;

namespace WebApi.UseCases.Commands.Matches.ApproveJoinRequest;

public class ApproveJoinRequestCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHubContext<MatchHub> _matchHubContext;
    private readonly ICurrentAccount _currentAccount;

    public ApproveJoinRequestCommandHandler(
        IApplicationDbContext dbContext,
        IHubContext<MatchHub> matchHubContext,
        ICurrentAccount currentAccount)
    {
        _dbContext = dbContext;
        _matchHubContext = matchHubContext;
        _currentAccount = currentAccount;
    }

    public async Task<ApproveJoinRequestCommandResponse?> Handle(ApproveJoinRequestCommand request, CancellationToken cancellationToken)
    {
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => m.MatchId == request.MatchId, CancellationToken.None);
        if (match is null)
        {
            await _matchHubContext.Clients.Client(request.ApproverConnectionId).SendAsync("ReceiveErrorAsync", new Notification("Match to Approve Join Request was not found.", "Matches.NotFound"));
            return null;
        }

        var approverIsNotOnMatch = !await _dbContext.Participants.AnyAsync(p => p.AccountId == _currentAccount.AccountId &&
                                                                                               p.MatchId == request.MatchId,
                                                                                               cancellationToken);
        if (approverIsNotOnMatch)
        {
            await _matchHubContext.Clients.Client(request.ApproverConnectionId)
                .SendAsync("ReceiveErrorAsync", new Notification("You can't approve another user Join Request.", "Matches.CannotApproveJoinRequest"));
            return null;
        }
        
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == request.RequesterAccountId, CancellationToken.None);
        if (account is null)
        {
            await _matchHubContext.Clients.Group(match.MatchId.ToString())
                .SendAsync("ReceiveErrorAsync", new Notification("Account to Approve Join Request was not found.", "Accounts.NotFound"));
            return null;
        }

        if (request.HasApproved)
        {
            await _matchHubContext.Clients.Client(request.RequesterConnectionId).SendAsync("ApproveJoinRequest");
            return new ApproveJoinRequestCommandResponse();
        }

        await _matchHubContext.Clients.Client(request.RequesterConnectionId).SendAsync("RejectJoinRequest");
        return new ApproveJoinRequestCommandResponse();
    }
}