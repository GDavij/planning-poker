using System.Net;
using Application.Abstractions.SignalR;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Stories.VoteStory;

public record VoteStoryCommandResponse(long StoryId, short Points);

public class VoteStoryCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ICurrentAccount _currentAccount;
    private readonly IMatchSignalRIntegrationIntegrationClient _matchSignalRIntegrationIntegrationClient;

    public VoteStoryCommandHandler(IApplicationDbContext dbContext, INotificationService notificationService, ICurrentAccount currentAccount, IMatchSignalRIntegrationIntegrationClient matchSignalRIntegrationIntegrationClient)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _currentAccount = currentAccount;
        _matchSignalRIntegrationIntegrationClient = matchSignalRIntegrationIntegrationClient;
    }

    public async Task<VoteStoryCommandResponse?> Handle(long matchId, long storyId, short points)
    {
        var match = await _dbContext.Matches.Include(m => m.Stories)
                                                .ThenInclude(s => s.StoryPoints)
                                            .Include(m => m.Participants)
                                            .FirstAsync(m => m.MatchId == matchId);

        var story = match.GetStoryWithId(storyId);
        if (story is null)
        {
            _notificationService.AddNotification("Could not find Story to vote.", "Story.NotFound", HttpStatusCode.NotFound);
            return null;
        }

        var participant = await _dbContext.Participants.FirstAsync(p => p.AccountId == _currentAccount.AccountId);

        StoryPoint gottenVote;
        if (story.HasVoteOf(participant))
        {
            gottenVote = story.Revote(points, participant);
        }
        else
        {
            gottenVote = story.Vote(points, participant);
        }


            
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        if (match.HaveAllParticipantsThatAreNotSpectatorsVotedFor(story))
        {
            var administratorConnectionId = await _dbContext.Participants.Where(p => p.AccountId == match.AccountId)
                                                                         .Select(p => p.SignalRConnectionId).FirstAsync();

            if (administratorConnectionId is null)
            {
                _notificationService.AddNotification("Unexpected Error, Admin has not connected to party hub.", "PartyHub.MissingAdmin", HttpStatusCode.Conflict);
                return null;
            }

            await _matchSignalRIntegrationIntegrationClient.NotifyAllParticipantsVotedForStoryAsync(story);
        };


        await _matchSignalRIntegrationIntegrationClient.NotifyStoryVoteAsync(gottenVote);
        return new VoteStoryCommandResponse(story.StoryId, gottenVote.Points);
    }
}