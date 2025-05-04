using System.Net;
using AspNetCore.Presentation.Models;
using Domain.Abstractions.Notification;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Presentation.Factories;

public static class ValidationResponseFactory
{
    public static IActionResult HandleInvalidModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(ms => ms.Value?.Errors.Any() ?? false)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => new Notification(
                x.ErrorMessage,
                "Request.InvalidDefinitionOfData",
                HttpStatusCode.BadRequest
            ));

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Notifications = errors,
            Meta = new MetaData(
                context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>(),
                context.HttpContext.RequestServices.GetRequiredService<TelemetryClient>()
            )
        };

        return new BadRequestObjectResult(response);
    }
}
