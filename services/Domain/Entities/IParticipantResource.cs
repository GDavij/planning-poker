namespace Domain.Entities;

public interface IParticipantResource
{
    public long AccountId { get; set; }
    public long MatchId { get; set; }
    public Participant Participant { get; set; }
}