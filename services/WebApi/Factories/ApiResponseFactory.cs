using Domain.Abstractions;
using WebApi.Models;

namespace WebApi.Factories;

public static class ApiResponseFactory
{
    public static ApiResponse<T> FromNotifications<T>(
        INotificationService notificationService,
        T? data,
        MetaData meta)
    {
        return new ApiResponse<T>
        {
            Success = !notificationService.HasNotifications(),
            Data = data,
            Notifications = notificationService.GetNotifications(),
            Meta = meta
        };
    }
}