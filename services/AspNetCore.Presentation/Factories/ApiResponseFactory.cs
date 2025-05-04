using AspNetCore.Presentation.Models;
using Domain.Abstractions;

namespace AspNetCore.Presentation.Factories;

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