using AspnetCore.Observability.Middlewares;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using WebApi.ApplicationInsights.Telemetries;

namespace AspnetCore.Observability;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationInsightsTelemetryAndLogging(this IServiceCollection services)
    {
        // Previous Application Insights configuration remains the same
        services.AddApplicationInsightsTelemetry(/* ... */);

        // Add HTTP context accessor for IP address access
        services.AddHttpContextAccessor();

        // Configure custom telemetry initializer
        services.AddSingleton<ITelemetryInitializer, IpAddressTelemetry>();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();

            logging.AddConsole();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                logging.AddDebug();
            }

            logging.AddApplicationInsights();
            
            // Configure logging with IP enrichment
            logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
            logging.AddFilter<ApplicationInsightsLoggerProvider>("System", LogLevel.Warning);
        });

        services.AddScoped<RequestLoggingMiddleware>();
        
        return services;
    }
    
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}