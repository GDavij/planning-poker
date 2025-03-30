namespace Domain.Abstractions;

public interface INotificationService
{
    public void AddNotification(string message, string code);
    public bool HasNotifications();

    public IEnumerable<Notification> GetNotifications();

}