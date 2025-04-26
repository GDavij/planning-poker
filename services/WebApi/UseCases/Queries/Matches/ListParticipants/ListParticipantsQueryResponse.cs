namespace WebApi.UseCases.Queries.Matches.ListParticipants;

public class ListParticipantsQueryResponse
{
    public long AccountId { get; init; }
    public string RoleName { get; init; }
    public bool IsSpectating { get; init; }
    public string ParticipantName { get; init; }
    public List<VoteWithoutPoints> Votes { get; init; }
}


public record VoteWithoutPoints(long StoryId, bool HasVotedAlready);