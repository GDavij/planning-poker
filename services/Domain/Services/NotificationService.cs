using Domain.Abstractions;

namespace Domain.Services;

public class NotificationService : INotificationService
{
    private readonly LinkedList<Notification> _notifications = [];
    
    public void AddNotification(string message, string code)
    {
        _notifications.AddLast(new Notification(message, code));
    }

    public bool HasNotifications()
    {
        return _notifications.Count > 0;
    }

    public IEnumerable<Notification> GetNotifications()
    {
        return _notifications;
    }
}