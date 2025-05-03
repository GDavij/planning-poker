using Application.Abstractions.SignalR;
using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Stories.ListStories;
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

    public Task JoinParticipantToMatchAsync(Participant participant, Match match)
    {
        throw new NotImplementedException();
    }
}