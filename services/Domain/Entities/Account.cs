namespace Domain.Entities;

public class Account
{
    public long AccountId { get; init; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public bool Deleted { get; private set; }
    public bool IsAdmin { get; private set; }
    
    public ICollection<Match> Matches { get; init; } = [];
    public ICollection<Participant> Participants { get; init; } = [];
}