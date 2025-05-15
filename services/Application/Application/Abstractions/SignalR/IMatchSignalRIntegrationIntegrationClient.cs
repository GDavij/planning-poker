using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Stories.ListStories;
using Domain.Abstractions;
using Domain.Abstractions.SignalR;
using Domain.Entities;

namespace Application.Abstractions.SignalR;

public interface IMatchSignalRIntegrationIntegrationClient : ISignalRIntegrationClient
{
    Task NotifySelectStoryToVoteForMatchAsync(Story story);
    Task NotifyApproveJoinRequestForParticipantAsync(Participant participant);
    Task NotifyRejectJoinRequestForParticipantAsync(Participant participant);
    Task NotifyClosedMatchAsync(Match match);
    Task NotifyCurrentListOfParticipantsOfMatch(Match currentMatch, IEnumerable<ListParticipantsQueryResponse> participants);
    Task NotifyCurrentListOfStoriesForMatchAsync(Match currentMatch, IEnumerable<ListStoriesQueryResponse> stories);
    Task NotifyStoryVoteAsync(StoryPoint storyPoint);
    Task NotifyAllParticipantsVotedForStoryAsync(Story story);
    
    Task JoinParticipantToMatchAsync(Participant participant, Match match, INotificationService notificationService);
    
}