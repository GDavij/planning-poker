using System.Configuration;
using System.Net;
using System.Threading.RateLimiting;
using AspNetCore.Presentation.Models;
using AspNetCore.Security.Middlewares;
using AspNetCore.Security.RateLimiting;
using AspNetCore.Security.Settings;
using Domain.Abstractions.Notification;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddRateLimitingForClients(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Rate limit for regular HTTP endpoints
            options.AddPolicy(RateLimitingDefinitions.NormalHttpRequestsRateLimit, httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 20,
                        TokensPerPeriod = 20,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        QueueLimit = 5,
                        AutoReplenishment = true
                    }));

            // Separate policy for SignalR hub methods
            options.AddPolicy(RateLimitingDefinitions.WebSocketsHttpRequestsRateLimit, httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100, // Higher limit for SignalR
                        TokensPerPeriod = 30, // More tokens per period
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        QueueLimit = 10, // Larger queue for real-time communications
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, cancellation) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.Headers["Retry-After"] = "60";

                var httpContextAccessor = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
                var telemetryClient = context.HttpContext.RequestServices.GetRequiredService<TelemetryClient>();
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IRateLimiterAssemblyMarker>>();
                
                logger.LogError("TOO MANY REQUESTS from Ip: {remoteIpAddress} - POSSIBLE DDOS",  context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "THREAT");
                
                await context.HttpContext.Response.WriteAsJsonAsync(new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Notifications = [new Notification("Too Many Requests", "Requests.PossibleDDOS", HttpStatusCode.TooManyRequests)],
                    Meta = new MetaData(httpContextAccessor, telemetryClient)
                }, cancellation);
            };
        });
        
        return services;
    }

    public static IApplicationBuilder UseCustomRateLimiting(this IApplicationBuilder app)
    {
        app.UseRateLimiter();

        return app;
    }

    public static IServiceCollection AddFirebaseJwtValidation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
            {
                jwt.Authority = configuration.GetValue<string>("Firebase:Auth:TokenAuthorityUrl");
                jwt.Audience = configuration.GetValue<string>("Firebase:Auth:ProjectId");
                jwt.TokenValidationParameters.ValidIssuer =
                    configuration.GetValue<string>("Firebase:Auth:ValidIssuerUrl");


                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue("Authorization", out var token))
                        {
                            ctx.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IServiceCollection AddCorsSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CorsSettings>(configuration.GetSection("Cors"));
        
        services.AddCors(options =>
        {
            var corsSettings = services.BuildServiceProvider().GetRequiredService<IOptions<CorsSettings>>().Value;
            
            options.AddDefaultPolicy(policy =>
            {
        
                policy.WithOrigins(corsSettings.Origins) // Adjust based on frontend URL
                    .WithHeaders(corsSettings.Headers)
                    .WithMethods(corsSettings.Methods)
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddErrorHandlerMiddleware(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlerMiddleware>();

        return services;
    }
    
    public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();

        return app;
    }
}

