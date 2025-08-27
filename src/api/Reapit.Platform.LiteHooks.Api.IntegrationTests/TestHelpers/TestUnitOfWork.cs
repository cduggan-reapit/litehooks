using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Data.Services;

namespace Reapit.Platform.LiteHooks.Api.IntegrationTests.TestHelpers;

/// <summary>Unit of work service for test implementation.</summary>
/// <param name="context">The test environment database context.</param>
/// <remarks>
/// The disposal of the main DbContext service causes problems with integration tests, so we swap it out for a version
/// that does not implement IDisposable or IAsyncDisposable, allowing the test to manage the lifetime of ApiDbContext. 
/// </remarks>
public class TestUnitOfWork(ApiDbContext context) : IUnitOfWork
{
    private IExamplesRepository? _examples;
    
    public IExamplesRepository Examples => _examples ??= new ExamplesRepository(context);
    
    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}