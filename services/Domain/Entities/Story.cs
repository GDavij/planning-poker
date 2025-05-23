using Domain.Abstractions;
using Domain.Abstractions.DataAccess;

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

    private Story()
    { }

    public Story(Match match, string name, string? storyNumber, short order)
    {
        Match = match;
        Name = name;
        StoryNumber = storyNumber;
        Order = order;
    }

    public void MoveTo(short order)
    {
        Order = order;
    }

    public void RenameTo(string name)
    {
        Name = name;
    }
    
    public void TagStoryWithNumber(string? storyNumber)
    {
        StoryNumber = storyNumber;
    }

    public bool HasVoteOf(Participant participant)
    {
        return StoryPoints.Any(p => p.AccountId == participant.AccountId && p.StoryId == StoryId);
    }

    public StoryPoint Revote(short points, Participant participant)
    {
        var storyPoint = StoryPoints.First(p => p.AccountId == participant.AccountId);
        storyPoint.RevaliateComplexityTo(points);

        return storyPoint;
    }
    
    public StoryPoint Vote(short points, Participant participant)
    {
        var storyPoint = new StoryPoint(this, points, participant);
        StoryPoints.Add(storyPoint);

        return storyPoint;
    }
}