using System.Net;
using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.SignalR;
using Domain.Entities;

namespace Application.SignalR;

public class MatchSignalRIntegrationIntegrationClient : IMatchSignalRIntegrationIntegrationClient
{
    private readonly ISignalRService<IMatchSignalRIntegrationIntegrationClient> _signalRService;

    public MatchSignalRIntegrationIntegrationClient(ISignalRService<IMatchSignalRIntegrationIntegrationClient> signalRService)
    {
        _signalRService = signalRService;
    }

    public Task NotifySelectStoryToVoteForGroupAsync(string groupId, Story story)
    {
        return _signalRService.SendAsyncForGroup(groupId, "SelectStoryToVoteAs", story.StoryId);
    }

    public Task NotifySelectStoryToVoteForMatchAsync(Story story)
    {
        throw new NotImplementedException();
    }

    public Task NotifyApproveJoinRequestForParticipantAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public Task NotifyRejectJoinRequestForParticipantAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public Task NotifyClosedMatchAsync(Match match)
    {
        throw new NotImplementedException();
    }

    public Task NotifyCurrentListOfParticipantsOfMatch(Match currentMatch, IEnumerable<ListParticipantsQueryResponse> participants)
    {
        throw new NotImplementedException();
    }

    public Task NotifyCurrentListOfStoriesForMatchAsync(Match currentMatch, IEnumerable<ListStoriesQueryResponse> stories)
    {
        throw new NotImplementedException();
    }

    public Task NotifyStoryVoteAsync(StoryPoint storyPoint)
    {
        throw new NotImplementedException();
    }

    public Task NotifyAllParticipantsVotedForStoryAsync(Story story)
    {
        throw new NotImplementedException();
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