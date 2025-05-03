using System.Text.Json.Serialization;
using Domain.Abstractions.Notification;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace WebApi.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init ; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Notification>? Notifications { get; init; }
    public MetaData Meta { get; init; }
}

public class MetaData
{
    public string TraceId { get; private set; }
    public string OperationId { get; private set; }
    public DateTime Timestamp { get; private set; }

    public MetaData(IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
    {
        var requestTelemetry = httpContextAccessor.HttpContext?.Features.Get<RequestTelemetry>();
        
        TraceId = requestTelemetry?.Context.Operation.Id ?? string.Empty;
        OperationId = requestTelemetry?.Context.Operation.ParentId ?? string.Empty;
        Timestamp = DateTime.UtcNow;
    }
}
