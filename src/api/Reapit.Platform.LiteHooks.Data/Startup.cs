using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Data.Services;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Data;

/// <summary>Extension methods for configuration associated with the Data layer.</summary>
[ExcludeFromCodeCoverage]
public static class Startup
{
    /// <summary>Add data-layer services to the application builder.</summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>A reference to the application builder after the services have been added.</returns>
    public static IHostApplicationBuilder AddDataServices(this IHostApplicationBuilder builder)
    {
        /*
         * There's some important stuff to know about how these services are registered:
         *  - The database context is registered with the reader connection.
         *  - Any service into which the context is injected using DI will therefore use the reader connection.
         *  - The database context is scoped, so the same context will be shared among services in the same request.
         *  - We have a custom factory implementation to spawn instances for the unit of work service.
         *  - The custom factory is a singleton - it has no state, so needs not be request scoped.
         *  - Anything using a context from the factory MUST manage the lifetime of the contexts it creates
         *      - this could be achieved through implementing IDisposable or a using statement.
         *  - We do not use AddDbContextFactory for the factory service, as this would cause conflict with the ef core
         *    migrations.  That's an internal decision, but keeping that as "out-of-the-box" as possible was preferred.
         *  - You'll likely need to either switch the default connection string from "Reader" to "Writer" to apply
         *    migrations, or temporarily put the Writer connection string in the Reader key in the configuration.
         *      - PR reviewers should keep an eye out for these changes being committed accidentally.
         */
        builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Reader")));
        builder.Services.AddSingleton<IApiDbContextFactory, ApiDbContextFactory>();

        // We register the unit of work service to allow people to perform write operations
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // We register the read only repositories to allow people to access them directly
        builder.Services.AddScoped<IReadOnlyExamplesRepository, ReadOnlyExamplesRepository>();

        return builder;
    }
}