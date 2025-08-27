namespace Reapit.Platform.LiteHooks.Api.Infrastructure.Logging;

/// <summary>Object describing configurable telemetry options.</summary>
public class TelemetryConfiguration
{
    /// <summary>The URL paths to exclude from tracing (note: logs will still be surfaced for these routes).</summary>
    public IEnumerable<string> ExcludedPaths { get; set; } = ["/swagger/*", "/api/health"];

    /// <summary>The request headers to include in trace records.</summary>
    public IEnumerable<string> RequestHeaders { get; set; } = ["x-api-*", "reapit-*"];
    
    /// <summary>The response headers to include in trace records.</summary>
    public IEnumerable<string> ResponseHeaders { get; set; } = [];
}