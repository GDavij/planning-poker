using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Ports.SignalR;

[Authorize]
public class MatchHub : Hub<IMatchClient>, IMatchHub
{
    private readonly IMediator _mediator;

    public MatchHub(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public Task EstimateHistory()
    {
        throw new NotImplementedException();
    }

    public Task AddStory()
    {
        throw new NotImplementedException();
    }

    public Task UpdateStory()
    {
        throw new NotImplementedException();
    }

    public Task RemoveStory()
    {
        throw new NotImplementedException();
    }

    public Task StartMatch()
    {
        throw new NotImplementedException();
    }

    public Task EndMatch()
    {
        throw new NotImplementedException();
    }

    public async Task JoinMatch(long matchId)
    {
        
    }
    
    // private Task NotifyPlayerJoined()
    // { }
    //
    // private Task NotifyPlayerHasLostConnection()
    // { }
    //
    // private Task NotifyPlayerHasReconnected()
    // { }

}