namespace Application.SignalR.MatchHub;

public static class MatchHubHooks
{
    public static readonly string OnListMatchStories = "StoriesUpdated";
    public static readonly string OnMatchClosed = "MatchClosed";
    public static readonly string OnSelectedStoryToVote = "StorySelected";
    public static readonly string OnApproveJoinRequest = "JoinRequestApproved";
    public static readonly string OnParticipantJoin = "ParticipantJoined";
    public static readonly string OnParticipantVote = "ParticipantVoted";
    public static readonly string OnRejectedJoinRequest = "JoinRequestRejected";
    public static readonly string OnEveryoneVoted = "ParticipantsHaveVoted";
}