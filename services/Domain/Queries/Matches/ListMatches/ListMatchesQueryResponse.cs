namespace Domain.Queries.Matches.ListMatches;

public record ListMatchesQueryResponse
{
    public long MatchId { get; init; }
    public string Description { get; init; }
    public bool HasStarted { get; init; }
    public bool HasClosed { get; init; }
};