using Application.Abstractions.SignalR;
using Domain.Abstractions.SignalR;
using Microsoft.AspNetCore.SignalR;
using WebApi.Ports.SignalR;

namespace WebApi.SignalR;

public class MatchSignalRServiceClient : ISignalRService<IMatchSignalRIntegrationIntegrationClient>
{
    private readonly IHubContext<MatchHub> _hubContext;

    public MatchSignalRServiceClient(IHubContext<MatchHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendAsyncForClient(string clientId, string methodName, params object[] arguments)
    {
        return _hubContext.Clients.Client(clientId).SendAsync(methodName, arguments);   
    }

    public Task SendAsyncForGroup(string groupId, string methodName, params object[] arguments)
    {
        return _hubContext.Clients.Group(groupId).SendAsync(methodName, arguments);
    }

    public Task AddUserClientIdToGroup(string clientId, string groupId)
    {
        return _hubContext.Groups.AddToGroupAsync(clientId, groupId);
    }

    public Task PopUserClientIdFromGroup(string clientId, string groupId)
    {
        return _hubContext.Groups.RemoveFromGroupAsync(clientId, groupId);
    }
}