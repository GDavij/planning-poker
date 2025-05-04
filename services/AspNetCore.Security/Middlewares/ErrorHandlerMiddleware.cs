using System.Net;
using AspNetCore.Presentation.Factories;
using AspNetCore.Presentation.Models;
using Domain.Abstractions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Security.Middlewares;

public class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private readonly INotificationService _notificationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TelemetryClient _telemetryClient;

    public ErrorHandlerMiddleware(
        ILogger<ErrorHandlerMiddleware> logger,
        IHostEnvironment env,
        INotificationService notificationService,
        IHttpContextAccessor httpContextAccessor,
        TelemetryClient telemetryClient)
    {
        _logger = logger;
        _env = env;
        _notificationService = notificationService;
        _httpContextAccessor = httpContextAccessor;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode httpStatusCode = _notificationService.GetNotifications()
                                                            .FirstOrDefault()
                                                            ?.HttpStatusCode 
                                        ?? HttpStatusCode.InternalServerError;

        if (!_notificationService.HasNotifications())
        {
            var errorMessage = _env.IsDevelopment()
                ? exception.ToString()
                : "An unexpected error occurred.";
            
            _notificationService.AddNotification(
                errorMessage,
                "Server.InternalError",
                HttpStatusCode.InternalServerError);
        }

        // Track notifications in telemetry
        foreach (var notification in _notificationService.GetNotifications())
        {
            _telemetryClient.TrackEvent("BusinessValidation", new Dictionary<string, string>
            {
                { "statusCode", notification.HttpStatusCode.ToString() },
                { "code", notification.Code },
                { "message", notification.Message }
            });
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;

        var response = ApiResponseFactory.FromNotifications<object>(
            _notificationService,
            null, // No data for error responses
            new MetaData(_httpContextAccessor, _telemetryClient));

        await context.Response.WriteAsJsonAsync(response);
    }
}
