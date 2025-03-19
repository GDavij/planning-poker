namespace Domain.Entities;

public class Participant
{
    public long AccountId { get; init; }
    public Account Account { get; init; }
    public byte RoleId { get; init; }
    public Role Role { get; init; }
    public long MatchId { get; init; }
    public Match Match { get; init; }
    public bool IsSpectating { get; init; }
    
    public ICollection<StoryPoint> StoryPoints { get; init; }
}