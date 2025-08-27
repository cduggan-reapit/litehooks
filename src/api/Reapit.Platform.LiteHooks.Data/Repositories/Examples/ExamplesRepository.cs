using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Examples;

/// <summary>A repository of <see cref="ExampleEntity"/> instances, capable of read and write operations.</summary>
/// <param name="context">The database context</param>
public sealed class ExamplesRepository(ApiDbContext context)
    : ReadOnlyExamplesRepository(context), IExamplesRepository
{
    /// <inheritdoc />
    public async Task<ExampleEntity> CreateAsync(ExampleEntity entity, CancellationToken cancellationToken)
    {
        await Context.Examples.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public Task<ExampleEntity> UpdateAsync(ExampleEntity entity, CancellationToken cancellationToken)
    {
        Context.Examples.Update(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public Task<ExampleEntity> DeleteAsync(ExampleEntity entity, CancellationToken cancellationToken)
    {
        Context.Examples.Remove(entity);
        return Task.FromResult(entity);
    }
}