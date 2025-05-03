namespace Domain.Abstractions.SignalR;

public interface ISignalRService<T> 
    where T : ISignalRIntegrationClient
{
    Task SendAsyncForClient(string clientId, string methodName, params object[] arguments);
    Task SendAsyncForGroup(string groupId, string methodName, params object[] arguments);

    Task AddUserClientIdToGroup(string clientId, string groupId);
    Task PopUserClientIdFromGroup(string clientId, string groupId);
}