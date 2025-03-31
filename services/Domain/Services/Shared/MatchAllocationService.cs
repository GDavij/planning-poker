using System.Collections.Immutable;
using Domain.Abstractions;
using Domain.Abstractions.Shared;
using Domain.Entities;

namespace Domain.Services.Shared;

public class MatchAllocationService : IMatchAllocationService
{
    private IImmutableDictionary<long, MatchRoom> _runningMatches = ImmutableDictionary<long, MatchRoom>.Empty;

    public bool HasMatchAllocated(Match match)
    {
        return _runningMatches.ContainsKey(match.MatchId);
    }

    public MatchRoom? GetRoom(Match match, INotificationService notificationService)
    {
        if (_runningMatches.TryGetValue(match.MatchId, out MatchRoom? room))
        {
            return room;
        }

        notificationService.AddNotification($"Could not get allocation with Id {match.MatchId}", "MatchAllocation.NotFoundMatch");
        return default;
    }

    public MatchRoom? AllocateRoomFor(Match match, INotificationService notificationService)
    {
        if (_runningMatches.ContainsKey(match.MatchId))
        {
            notificationService.AddNotification($"Room for match with Id {match.MatchId} already allocated.", "MatchAllocation.MatchAlreadyRunning");
            return default;
        }

        var room = new MatchRoom(match);
        
        ImmutableInterlocked.Update(ref _runningMatches, rooms => rooms.SetItem(match.MatchId, room));
        return room;
    }
}