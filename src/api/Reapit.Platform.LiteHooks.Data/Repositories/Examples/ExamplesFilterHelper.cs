using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Examples;

/// <summary>Filter helper for <see cref="ExampleEntity"/> queries.</summary>
public static class ExamplesFilterHelper
{
    /// <summary>Applies a name filter to the collection.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The value by which to filter.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ExampleEntity> ApplyNameFilter(this IQueryable<ExampleEntity> queryable, string? value)
        => value is null ? queryable : queryable.Where(entity => entity.Name.Contains(value));

    /// <summary>Applies a description filter to the collection.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The value by which to filter.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ExampleEntity> ApplyDescriptionFilter(this IQueryable<ExampleEntity> queryable, string? value)
        => value is null ? queryable : queryable.Where(entity => entity.Description != null && entity.Description.Contains(value));
}