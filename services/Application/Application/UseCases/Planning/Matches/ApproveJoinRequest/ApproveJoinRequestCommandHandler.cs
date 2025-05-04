using System.ComponentModel.DataAnnotations;
using System.Net;
using Application.Abstractions.SignalR;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Matches.ApproveJoinRequest;

public record ApproveJoinRequestCommand
{
    [Required(ErrorMessage = "Missing Confirmation about Approval")]
    public bool HasApproved { get; init; }
    
    [Required(ErrorMessage = "Missing Entry Requester")]
    [Range(1, long.MaxValue, ErrorMessage = "Entry Requester must be valid")]
    public long RequesterAccountId { get; init; }
    
    [Required(ErrorMessage = "Match Id is Required")]
    [Range(1, long.MaxValue, ErrorMessage = "Match Id is not valid")]
    public long MatchId { get; init; }
}

public class ApproveJoinRequestCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;
    private readonly ICurrentAccount _currentAccount;
    private readonly INotificationService _notificationService;

    public ApproveJoinRequestCommandHandler(
        IApplicationDbContext dbContext,
        IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient,
        ICurrentAccount currentAccount,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
        _currentAccount = currentAccount;
        _notificationService = notificationService;
    }

    public async Task Handle(ApproveJoinRequestCommand request)
    {
        var match = await _dbContext.Matches.FirstOrDefaultAsync(m => m.MatchId == request.MatchId);
        if (match is null)
        {
            _notificationService.AddNotification("Match to Approve Join Request was not found.", "Matches.NotFound", HttpStatusCode.NotFound);
            return;
        }

        var approverIsNotOnMatch = !await _dbContext.Participants.AnyAsync(p => p.AccountId == _currentAccount.AccountId &&
                                                                                               p.MatchId == request.MatchId);
        if (approverIsNotOnMatch)
        {
            //TODO: Refactor to allow a notification for administrator
            _notificationService.AddNotification("You can't approve another user Join Request.", "Matches.CannotApproveJoinRequest", HttpStatusCode.NotFound);
            return;
        }
        
        var participant = await _dbContext.Participants.FirstOrDefaultAsync(p => p.AccountId == request.RequesterAccountId);
        if (participant is null)
        {
            // Maybe notify this to everybody in the party
            _notificationService.AddNotification("Account to Approve Join Request was not found.", "Accounts.NotFound", HttpStatusCode.NotFound);
            return;
        }

        if (request.HasApproved)
        {
            await _matchSignalRIntegrationIntegrationClient.NotifyApproveJoinRequestForParticipantAsync(participant);
            return;
        }

        await _matchSignalRIntegrationIntegrationClient.NotifyRejectJoinRequestForParticipantAsync(participant);
    }
}