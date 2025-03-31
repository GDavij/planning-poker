using Domain.Entities;

namespace Domain.Abstractions;

public class MatchRoom
{
    public Match Match { get; init; }
    public LinkedList<Participant> Participants { get; init; } = [];

    public MatchRoom(Match match)
    {
        Match = match;
    }

    public void Join(Participant participant)
    {
        Participants.AddLast(participant);
    }

    public void CloseSessionFor(Participant participant)
    {
        Participants.Remove(participant);
    }
}