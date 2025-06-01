export enum SignalRHooks {
  OnListMatchStories = "StoriesUpdated",
  OnMatchClosed = "MatchClosed",
  OnSelectedStoryToVote = "StorySelected",
  OnApproveJoinRequest = "JoinRequestApproved",
  OnParticipantJoin = "ParticipantJoined",
  OnParticipantVote = "ParticipantVoted",
  OnRejectedJoinRequest = "JoinRequestRejected",
  OnEveryoneVoted = "ParticipantsHaveVoted",
}
