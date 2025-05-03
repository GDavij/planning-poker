using System.Configuration;
using System.Threading.RateLimiting;
using AspNetCore.Security.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        });

        return services;
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
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var origins = configuration.GetValue<string>("Cors:Origins") 
                              ?? throw new ConfigurationErrorsException("Cors:Origins");
        
                var headers = configuration.GetValue<string[]>("Cors:Headers") 
                              ?? throw new ConfigurationErrorsException("Configuration Cors:Headers does not have a valid value...");
        
                var methods = configuration.GetValue<string[]>("Cors:Headers") 
                              ?? throw new ConfigurationErrorsException("Configuration Cors:Headers does not have a valid value...");
        
                policy.WithOrigins(origins) // Adjust based on frontend URL
                    .WithHeaders(headers)
                    .WithMethods(methods)
                    .AllowCredentials();
            });
        });

        return services;
    }
    
}