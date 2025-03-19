namespace Domain.Entities;

public class StoryPoint
{
    public long StoryId { get; init; }
    public Story Story { get; init; }
    
    public long AccountId { get; init; }
    public long MatchId { get; init; }
    public Participant Participant { get; init; }
    
    public short Points { get; init; }
}