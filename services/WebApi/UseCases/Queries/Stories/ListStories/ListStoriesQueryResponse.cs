namespace WebApi.UseCases.Queries.Stories;

public record ListStoriesQueryResponse
{
    public long StoryId { get; init; }
    public long MatchId { get; init; }
    public string Name { get; init; }
    public string? StoryNumber { get; init; }
    public short Order { get; init; }
    public List<StoryPointResponse> StoryPoints { get; init; }
};

public record StoryPointResponse(short Points, string ParticipantName);