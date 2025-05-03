using System.Net;
using Domain.Abstractions;

namespace Application.Notification;

public class NotificationService : INotificationService
{
    private readonly LinkedList<Domain.Abstractions.Notification.Notification> _notifications = [];
    
    public void AddNotification(string message, string code, HttpStatusCode httpStatusCode)
    {
        _notifications.AddLast(new Domain.Abstractions.Notification.Notification(message, code, httpStatusCode));
    }

    public bool HasNotifications()
    {
        return _notifications.Count > 0;
    }

    public IEnumerable<Domain.Abstractions.Notification.Notification> GetNotifications()
    {
        return _notifications;
    }
}