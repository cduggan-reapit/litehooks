using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;

namespace Reapit.Platform.LiteHooks.Data.Services;

/// <summary>
/// Context within which read/write operations may be performed. All repositories accessed within a UnitOfWork will
/// share a database context. 
/// </summary>
/// <param name="factory">The database context factory.</param>
/// <remarks>
/// As this service is responsible for managing the database contexts that it initializes, we implement the disposable
/// interfaces to ensure those contexts are disposed with the request scope.
/// </remarks>
public class UnitOfWork(IApiDbContextFactory factory) : IUnitOfWork, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Backing field for <see cref="Context"/>, this allows us to delay context creation until we need it but should
    /// never be referenced directly outside of that property definition.
    /// </summary>
    private ApiDbContext? _context;

    /// <summary>The database context for this unit of work.</summary>
    private ApiDbContext Context => _context ??= factory.CreateDbContext(ContextType.ReadWrite);

    /// <summary>Backing field for <see cref="Examples"/>.</summary>
    private IExamplesRepository? _examplesRepository;

    /// <inheritdoc />
    public IExamplesRepository Examples => _examplesRepository ??= new ExamplesRepository(Context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await Context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose() => _context?.Dispose();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_context != null)
            await _context.DisposeAsync();
    }
}