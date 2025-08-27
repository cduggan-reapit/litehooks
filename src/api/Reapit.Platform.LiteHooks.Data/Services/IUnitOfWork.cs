using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Services;

/// <summary>Describes the service responsible for managing database transactions.</summary>
public interface IUnitOfWork
{
    /// <summary>Accessor for the <see cref="ExampleEntity"/> repository.</summary>
    public IExamplesRepository Examples { get; }

    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}