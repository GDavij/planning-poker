using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;

namespace WebApi.UseCases.Commands.Stories.VoteStory;

public class VoteStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly ICurrentAccount _currentAccount;

    public VoteStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, IHubContext<MatchHub> hubContext, ICurrentAccount currentAccount)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _currentAccount = currentAccount;
    }

    public async Task Handle(long matchId, long storyId, short points)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories)
                                                .ThenInclude(s => s.StoryPoints)
                                            .Include(m => m.Participants)
                                            .FirstAsync(m => m.MatchId == matchId);

        var story = match.GetStoryWithId(storyId);
        if (story is null)
        {
            _notificationService.AddNotification("Could not find Story to vote.", "Story.NotFound");
            return;
        }

        var participant = await _dbContext.Participants.FirstAsync(p => p.AccountId == _currentAccount.AccountId);

        if (story.HasVoteOf(participant))
        {
            story.Revote(points, participant);
        }
        else
        {
            story.Vote(points);
        }
            
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        if (match.HaveAllParticipantsThatAreNotSpectatorsVotedFor(story))
        {
            var administratorConnectionId = await _dbContext.Participants.Where(p => p.AccountId == match.AccountId)
                                                                         .Select(p => p.SignalRConnectionId).FirstAsync();

            if (administratorConnectionId is null)
            {
                _notificationService.AddNotification("Unexpected Error, Admin has not connected to party hub.", "PartyHub.MissingAdmin");
                return;
            }
            
            await _hubContext.Clients.Client(administratorConnectionId)
                             .SendAsync("AllParticipantsVotedForStoryWithId", story.StoryId);
        };

        await _hubContext.Clients.Group(match.MatchId.ToString()).SendAsync("SomeoneVoted", participant.AccountId);
    }
}