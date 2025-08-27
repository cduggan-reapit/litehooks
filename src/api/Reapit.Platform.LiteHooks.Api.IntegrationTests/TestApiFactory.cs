using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Testing.Extensions;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Services;
using System.Data.Common;

namespace Reapit.Platform.LiteHooks.Api.IntegrationTests;

public class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Replace services
        builder.ConfigureServices(services =>
        {
            // Remove the default ApiDbContext configuration and set up an in-memory Sqlite provider.
            services.RemoveServiceForType<DbContextOptions<ApiDbContext>>();
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });
            services.AddDbContext<ApiDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
                options.EnableSensitiveDataLogging();
            });

            // Swap the UnitOfWork implementation for one which doesn't manage context lifetime
            services.RemoveServiceForType<IUnitOfWork>();
            services.AddScoped<IUnitOfWork, TestUnitOfWork>();
            
            // Remove the ApiDbContextFactory, since it's not used.
            services.RemoveServiceForType<IApiDbContextFactory>();
        });

        builder.UseEnvironment("Testing");
    }
}