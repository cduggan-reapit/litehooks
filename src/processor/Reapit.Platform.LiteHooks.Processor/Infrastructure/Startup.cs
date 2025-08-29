using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reapit.Platform.LiteHooks.Processor.Services;
using Reapit.Platform.Common;

namespace Reapit.Platform.LiteHooks.Processor.Infrastructure;

/// <summary>Service container extension methods.</summary>
public static class Startup
{
    /// <summary>Add required services to the service collection.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
        => services.AddLogging(logging => logging.AddConsole())
            .AddCommonServices()
            .AddTransient<ISqsEventProcessor, SqsEventProcessor>()
            .AddTransient<ISqsMessageProcessor, SqsMessageProcessor>();
}