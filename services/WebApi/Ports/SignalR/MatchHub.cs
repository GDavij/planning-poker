using Application.UseCases.Planning.Matches.ApproveJoinRequest;
using Application.UseCases.Planning.Matches.JoinMatch;
using AspNetCore.Security.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Ports.SignalR;

[Authorize]
[EnableRateLimiting(RateLimitingDefinitions.WebSocketsHttpRequestsRateLimit)]
public sealed class MatchHub : Hub
{
    private readonly IServiceProvider _sp;

    public MatchHub(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task JoinMatchAsync(long matchId)
    {
        var handler = _sp.GetRequiredService<JoinMatchCommandHandler>();
        await handler.Handle(new JoinMatchCommand(matchId, Context.ConnectionId), CancellationToken.None);
    }

    public async Task ApproveMatchAsync(ApproveJoinRequestCommand command)
    {
        command = command with
        {
            ApproverConnectionId = Context.ConnectionId
        };

        var handler = _sp.GetRequiredService<ApproveJoinRequestCommandHandler>();
        await handler.Handle(command);
    }
}