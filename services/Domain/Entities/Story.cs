namespace Domain.Entities;

public class Story
{
    public long StoryId { get; init; }
    public long MatchId { get; init; }
    public Match Match { get; init; }
    public string Name { get; private set; }
    public string? StoryNumber { get; private set; }
    public short Order { get; private set; }

    public ICollection<StoryPoint> StoryPoints { get; private set; } = [];
}