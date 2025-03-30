namespace WebApi.Ports.SignalR;

public interface IMatchClient
{
    Task EstimateHistory();
    Task AddStory();
    Task UpdateStory();
    Task RemoveStory();
    Task StartMatch();
    Task EndMatch();
}