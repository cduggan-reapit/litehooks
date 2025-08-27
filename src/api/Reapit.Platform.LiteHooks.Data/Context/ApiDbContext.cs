using Microsoft.EntityFrameworkCore;
using Reapit.Platform.LiteHooks.Domain.Entities;
using System.Reflection;

namespace Reapit.Platform.LiteHooks.Data.Context;

/// <summary>The database context for this API.</summary>
/// <param name="options">The options for this context.</param>
public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    /// <summary>The example entity collection.</summary>
    public DbSet<ExampleEntity> Examples { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}