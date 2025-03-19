namespace Domain.Entities;

public class Match
{
    public long MatchId { get; init; }
    public long AccountId { get; init; }
    public Account Account { get; init; }
    public string Description { get; private set; }

    public ICollection<Participant> Participants { get; init; } = [];
    public ICollection<Story> Stories { get; init; } = [];
}