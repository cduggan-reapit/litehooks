using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
            
            if(builder.Environment.IsDevelopment())
                logging.AddConsoleExporter();
        });
        
        // When enabled, this configures OTel
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(traceBuilder => traceBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAWSInstrumentation())
            .WithMetrics(meterBuilder => meterBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAWSInstrumentation())
            .UseOtlpExporter();

        return builder;
    }
}