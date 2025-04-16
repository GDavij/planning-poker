using Domain.Abstractions.Auth.Models;

namespace Domain.Entities;

public class Match
{
    public long MatchId { get; init; }
    public long AccountId { get; init; }
    public Account Account { get; init; }
    public string Description { get; private set; }
    public bool HasStarted { get; private set; }
    public bool HasClosed { get; private set; }

    public ICollection<Participant> Participants { get; init; } = [];
    public ICollection<Story> Stories { get; init; } = [];
    
    private Match()
    { }

    public Match(ICurrentAccount account, string description)
    {
        AccountId = account.AccountId;
        Description = description;
    }

    public void Receive(Participant participant)
    {
        Participants.Add(participant);
    }

    public bool CheckParticipantIsAdmin(Participant participant)
    {
        return participant.AccountId == AccountId;
    }
}