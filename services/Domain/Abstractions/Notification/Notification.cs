using System.Net;

namespace Domain.Abstractions.Notification;

public record Notification(string Message, string Code, HttpStatusCode HttpStatusCode);