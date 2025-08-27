using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Core;

/// <summary>Extension methods for configuration associated with the Application layer.</summary>
[ExcludeFromCodeCoverage]
public static class Startup
{
    /// <summary>Add application-layer services to the application builder.</summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>A reference to the application builder after the services have been added.</returns>
    public static IHostApplicationBuilder AddCoreServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<UseCases.Examples.CreateExample.CreateExampleCommandHandler>());

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.Examples.CreateExample.CreateExampleCommandValidator));

        return builder;
    }
}