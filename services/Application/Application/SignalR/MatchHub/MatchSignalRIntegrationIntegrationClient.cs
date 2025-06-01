using System.Net;
using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.SignalR;
using Domain.Entities;

namespace Application.SignalR.MatchHub;

public class MatchSignalRIntegrationIntegrationClient : IMatchSignalRIntegrationIntegrationClient
{
    private readonly ISignalRService<IMatchSignalRIntegrationIntegrationClient> _signalRService;

    public MatchSignalRIntegrationIntegrationClient(ISignalRService<IMatchSignalRIntegrationIntegrationClient> signalRService)
    {
        _signalRService = signalRService;
    }

    public Task NotifySelectStoryToVoteForMatchAsync(Story story)
    {
        return _signalRService.SendAsyncForGroup(story.MatchId.ToString(), MatchHubHooks.OnSelectedStoryToVote);
    }

    public Task NotifyApproveJoinRequestForParticipantAsync(Participant participant)
    {
        return _signalRService.SendAsyncForClient(participant.SignalRConnectionId!, MatchHubHooks.OnApproveJoinRequest);
    }

    public Task NotifyRejectJoinRequestForParticipantAsync(Participant participant)
    {
        return _signalRService.SendAsyncForClient(participant.SignalRConnectionId!, MatchHubHooks.OnRejectedJoinRequest);
    }

    public Task NotifyClosedMatchAsync(Match match)
    {
        return _signalRService.SendAsyncForGroup(match.MatchId.ToString(), MatchHubHooks.OnMatchClosed);
    }

    public Task NotifyCurrentListOfParticipantsOfMatch(Match currentMatch, IEnumerable<ListParticipantsQueryResponse> participants)
    {
        return _signalRService.SendAsyncForGroup(currentMatch.MatchId.ToString(), MatchHubHooks.OnParticipantJoin);
    }

    public Task NotifyCurrentListOfStoriesForMatchAsync(Match currentMatch, IEnumerable<ListStoriesQueryResponse> stories)
    {
        return _signalRService.SendAsyncForGroup(currentMatch.MatchId.ToString(), MatchHubHooks.OnListMatchStories, stories);
    }

    public Task NotifyStoryVoteAsync(StoryPoint storyPoint)
    {
        return _signalRService.SendAsyncForGroup(storyPoint.MatchId.ToString(), MatchHubHooks.OnParticipantVote, storyPoint);
    }

    public Task NotifyAllParticipantsVotedForStoryAsync(Story story)
    {
        return _signalRService.SendAsyncForGroup(story.MatchId.ToString(), MatchHubHooks.OnEveryoneVoted, story);
    }

    public async Task JoinParticipantToMatchAsync(Participant participant, Match match, INotificationService notificationService)
    {
        if (string.IsNullOrWhiteSpace(participant.SignalRConnectionId))
        {
            notificationService.AddNotification("Participant is not connected to the Hub...", "Participan.NoHubConnection", HttpStatusCode.Gone);
        }
        
        await _signalRService.AddUserClientIdToGroup(participant.SignalRConnectionId!, match.MatchId.ToString());
        await _signalRService.SendAsyncForClient(participant.SignalRConnectionId!, "ApproveJoinRequest");
    }
}