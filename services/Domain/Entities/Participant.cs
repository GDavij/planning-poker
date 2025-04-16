using System.Runtime.Serialization;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;

namespace Domain.Entities;

public class Participant
{
    public long AccountId { get; init; }
    public Account Account { get; init; }
    public byte RoleId { get; private set; }
    public Role Role { get; private set; }
    public long MatchId { get; init; }
    public Match Match { get; init; }
    public bool IsSpectating { get; private set; }
    public string? SignalRConnectionId { get; private set; }
    
    private Participant()
    { }

    public Participant(ICurrentAccount currentAccount, Role role)
    {
        AccountId = currentAccount.AccountId;
        Role = role;
    }

    public bool Join(Match match, INotificationService notificationService)
    {
        if (Match is not null)
        {
            notificationService.AddNotification($"Already Participanting on match {match.Description}", "participant.alreadyJoinedMatch");
            return false;
        }

        match.Receive(this);
        return true;
    }

    public void SpectateMatch()
    {
        IsSpectating = true;
    }

    public void ActOverMatch()
    {
        IsSpectating = false;
    }

    public void ConnectedAt(string signalRConnectionId)
    {
        SignalRConnectionId = signalRConnectionId;
    }
    
    public ICollection<StoryPoint> StoryPoints { get; init; }
}