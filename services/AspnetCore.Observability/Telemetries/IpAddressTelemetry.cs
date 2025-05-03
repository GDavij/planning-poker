using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace WebApi.ApplicationInsights.Telemetries;

public class IpAddressTelemetry : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IpAddressTelemetry(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(ITelemetry telemetry)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Check if telemetry supports properties
            if (telemetry is ISupportProperties propertiesTelemetry)
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(clientIp))
                {
                    propertiesTelemetry.Properties["ClientIP"] = clientIp;
                }

                // Optionally add X-Forwarded-For if behind a proxy
                var forwardedIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedIp))
                {
                    propertiesTelemetry.Properties["X-Forwarded-For"] = forwardedIp;
                }
            }

            // For global properties that should apply to all telemetry
            var clientIpGlobal = context.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(clientIpGlobal))
            {
                telemetry.Context.GlobalProperties["ClientIP"] = clientIpGlobal;
            }
        }
    }
}

