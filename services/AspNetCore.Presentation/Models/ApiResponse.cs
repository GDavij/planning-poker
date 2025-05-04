using System.Diagnostics;
using System.Text.Json.Serialization;
using Domain.Abstractions.Notification;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Presentation.Models;

public class ApiResponse<T>
{
    public required bool Success { get; init; }
    public T? Data { get; init ; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Notification>? Notifications { get; init; }
    public required MetaData Meta { get; init; }
}

public class MetaData
{
    public string TraceId { get; private set; }
    public string OperationId { get; private set; }
    public string ParentId { get; private set; }
    public DateTime Timestamp { get; private set; }

    public MetaData(IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
    {
        var requestTelemetry = telemetryClient.Context.Operation;
        var activity = Activity.Current;

        TraceId = activity?.TraceId.ToString() ?? requestTelemetry.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty;
        OperationId = activity?.RootId ?? requestTelemetry.Id ?? string.Empty;
        ParentId = activity?.ParentId ?? requestTelemetry.ParentId ?? string.Empty;
        Timestamp = DateTime.UtcNow;
    }
}

