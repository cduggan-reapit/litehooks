using Reapit.Platform.LiteHooks.Data.Context;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Common.Abstract;

/// <summary>Base class implemented by repository implementations.</summary>
/// <param name="context"></param>
public abstract class RepositoryBase(ApiDbContext context)
{
    /// <summary>The repository database context.</summary>
    protected ApiDbContext Context { get; } = context;
}