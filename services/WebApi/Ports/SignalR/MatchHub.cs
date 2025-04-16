using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.UseCases.Commands.Matches.ApproveJoinRequest;
using WebApi.UseCases.Commands.Matches.JoinMatch;

namespace WebApi.Ports.SignalR;

[Authorize]
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
        await handler.Handle(command, CancellationToken.None);
    }
}