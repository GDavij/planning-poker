using System.Net;
using Domain.Abstractions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using WebApi.Factories;
using WebApi.Models;

namespace WebApi.Ports.Http.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected readonly INotificationService NotificationService;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly TelemetryClient TelemetryClient;

    protected BaseApiController(INotificationService notificationService, IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
    {
        NotificationService = notificationService;
        HttpContextAccessor = httpContextAccessor;
        TelemetryClient = telemetryClient;
    }
    
    protected IActionResult RespondWith<T>(
        T? data = default,
        HttpStatusCode successResponse = HttpStatusCode.OK)
    {
        var httpResult = successResponse;

        if (NotificationService.HasNotifications())
        {
            httpResult = NotificationService.GetNotifications().First().HttpStatusCode;

            foreach (var notification in NotificationService.GetNotifications())
            {
                TelemetryClient.TrackEvent("BusinessValidation", new Dictionary<string, string>
                {
                    { "code", notification.Code },
                    { "message", notification.Message }
                });
            }
        }

        var response = ApiResponseFactory.FromNotifications(
            NotificationService,
            data,
            new MetaData(HttpContextAccessor, TelemetryClient));

        return StatusCode((int)httpResult, response);
    }
}