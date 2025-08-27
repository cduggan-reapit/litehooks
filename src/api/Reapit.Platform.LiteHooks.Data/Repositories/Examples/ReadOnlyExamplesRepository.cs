using Microsoft.EntityFrameworkCore;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Common.Abstract;
using Reapit.Platform.LiteHooks.Data.Repositories.Common.Filters;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Examples;

/// <summary>A read-only repository containing <see cref="ExampleEntity"/> instances.</summary>
/// <param name="context">The database context.</param>
public class ReadOnlyExamplesRepository(ApiDbContext context) : RepositoryBase(context), IReadOnlyExamplesRepository
{
    /// <inheritdoc />
    public async Task<ExampleEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await Context.Examples.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<ExampleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken)
        => await Context.Examples.SingleOrDefaultAsync(e => e.Name == name, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<ExampleEntity>> GetAsync(
        string? name = null,
        string? description = null,
        DateFilter? dateFilter = null,
        PaginationFilter? pagination = null,
        CancellationToken cancellationToken = default)
        => await Context.Examples
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .ApplyCursor(pagination?.Cursor)
            .ApplyPagination(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);
}