using System.Net;

namespace Domain.Abstractions;

public interface INotificationService
{
    public void AddNotification(string message, string code, HttpStatusCode httpStatusCode);
    public bool HasNotifications();

    public IEnumerable<Notification.Notification> GetNotifications();

}