using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Infrastructure.Logging;

/// <summary>Extension methods for configuration of logging services and middleware.</summary>
[ExcludeFromCodeCoverage(Justification = "This is service configuration, not application logic.")]
public static class Startup
{
    /// <summary>Adds custom logging to the logging pipeline.</summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>A reference to the instance after the operation has been applied.</returns>
    public static IHostApplicationBuilder AddLoggingServices(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });


        var configuration = new TelemetryConfiguration();
        builder.Configuration.GetSection("Telemetry").Bind(configuration);
        
        // When enabled, this configures OTel
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(traceBuilder =>
            {
                traceBuilder.AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context => HttpRequestTraceFilter(context, configuration);
                        options.EnrichWithHttpRequest = (activity, request) => AppendHttpRequestData(activity, request, configuration);
                        options.EnrichWithHttpResponse = (activity, response) => AppendHttpResponseData(activity, response, configuration);
                    })
                    .AddHttpClientInstrumentation()
                    .AddAWSInstrumentation();
            })
            .WithMetrics(meterBuilder => meterBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAWSInstrumentation())
            .UseOtlpExporter();

        return builder;
    }
    
    // return true if collected, false to ignore
    private static bool HttpRequestTraceFilter(HttpContext httpContext, TelemetryConfiguration configuration)
    {
        var url = httpContext.Request.Path.Value;
        
        // If there's no URL, we'll want to log that
        if (string.IsNullOrWhiteSpace(url))
            return true;

        // If it's excluded, return false to suppress - otherwise return true
        var isExcluded = configuration.ExcludedPaths.Any(pattern => IsWildcardMatch(pattern, url));
        return !isExcluded;
    }
    
    private static void AppendHttpRequestData(Activity activity, HttpRequest request, TelemetryConfiguration configuration)
    {
        if (!activity.IsAllDataRequested)
            return;
        
        activity.SetTag("http.request.ip-address", request.HttpContext.Connection.RemoteIpAddress?.ToString());
        
        const string prefix = "http.response.header";
        var augmentHeaders = configuration.RequestHeaders.ToList();
        var headers = request.Headers.Where(header =>
            augmentHeaders.Any(pattern => IsWildcardMatch(pattern, header.Key)));
        
        foreach (var header in headers)
            activity.SetTag($"{prefix}.{header.Key}", header.Value);
    }
    
    private static void AppendHttpResponseData(Activity activity, HttpResponse request, TelemetryConfiguration configuration)
    {
        if (!activity.IsAllDataRequested)
            return;

        const string prefix = "http.response.header";
        var augmentHeaders = configuration.ResponseHeaders.ToList();
        var headers = request.Headers.Where(header =>
            augmentHeaders.Any(pattern => IsWildcardMatch(pattern, header.Key)));
        
        foreach (var header in headers)
            activity.SetTag($"{prefix}.{header.Key}", header.Value);
    }
    
    private static bool IsWildcardMatch(string pattern, string value)
    {
        const char wildcard = '*';
        const StringComparison comparer = StringComparison.OrdinalIgnoreCase;

        var comparison = pattern.Trim(wildcard);

        // pattern = */path/*
        if (pattern.StartsWith(wildcard) && pattern.EndsWith(wildcard))
            return value.Contains(comparison, comparer);

        // pattern = */path/
        if (pattern.StartsWith(wildcard))
            return value.EndsWith(comparison, comparer);

        // pattern = /path/*
        if (pattern.EndsWith(wildcard))
            return value.StartsWith(comparison, comparer);

        // Otherwise they have to be an exact match
        return value.Equals(pattern, comparer);
    }
}