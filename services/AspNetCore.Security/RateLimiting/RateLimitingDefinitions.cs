namespace AspNetCore.Security.RateLimiting;

public static class RateLimitingDefinitions
{
    public const string NormalHttpRequestsRateLimit = "http-client-tcp";
    public const string WebSocketsHttpRequestsRateLimit = "web-sockets-tcp";
}