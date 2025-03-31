namespace WebApi.Ports.SignalR;

public interface IMatchHub
{
    Task EstimateHistory();
    Task AddStory();
    Task UpdateStory();
    Task RemoveStory();
    Task StartMatch();
    Task EndMatch();
    Task JoinMatch();
}