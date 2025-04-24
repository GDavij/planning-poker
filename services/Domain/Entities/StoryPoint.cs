namespace Domain.Entities;

public class StoryPoint : IParticipantResource
{
    public long StoryId { get; init; }
    public Story Story { get; init; }
    
    public long AccountId { get; set; }
    public long MatchId { get; set; }
    public Participant Participant { get; set; }
    
    public short Points { get; private set; }

    public void RevaliateComplexityTo(short points)
    {
        Points = points;
    }

    private StoryPoint()
    { }

    public StoryPoint(Story story, short points)
    {
        Story = story;
        Points = points;
    }
}