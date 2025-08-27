using Reapit.Platform.Internal.Common.Database.Models;
using Reapit.Platform.Internal.Common.Database.Repositories;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Examples;

/// <summary>Describes a read-only repository containing instances of <see cref="ExampleEntity"/>.</summary>
public interface IReadOnlyExamplesRepository : IReadOnlyRepository<ExampleEntity>
{
    /// <summary>Get an instance of ExampleEntity by its unique name.</summary>
    /// <param name="name">The unique name of the instance to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<ExampleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>Get a page of <see cref="ExampleEntity"/> instances matching the given filters.</summary>
    /// <param name="name">Limit results to records where the name contains is equal to this value.</param>
    /// <param name="description">Limit results to records where the description contains this value.</param>
    /// <param name="dateFilter">Limit results to records matching this date filter.</param>
    /// <param name="pagination">Limit results to the page of records described by this filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IEnumerable<ExampleEntity>> GetAsync(
        string? name = null,
        string? description = null,
        DateFilter? dateFilter = null,
        PaginationFilter? pagination = null,
        CancellationToken cancellationToken = default);
}