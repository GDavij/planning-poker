using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspnetCore.Observability.Middlewares;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            
            using (_logger.BeginScope(new Dictionary<string, object>
                   {
                       ["ClientIP"] = clientIp ?? "unknown",
                       ["X-Forwarded-For"] = forwardedIp ?? "none",
                       ["Path"] = context.Request.Path,
                       ["Method"] = context.Request.Method
                   }))
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} request from IP: {ClientIP}",
                    context.Request.Method,
                    context.Request.Path,
                    clientIp);

                await next(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred processing the request");
            throw;
        }
    }
}
