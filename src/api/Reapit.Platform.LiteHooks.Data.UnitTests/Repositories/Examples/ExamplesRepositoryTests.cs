using Microsoft.EntityFrameworkCore;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Examples;

public static class ExamplesRepositoryTests
{
    public class CreateAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_AddTrackedEntity()
        {
            var entity = new ExampleEntity("new entity name", "new entity description");
            var sut = CreateSut(Context);
            _ = await sut.CreateAsync(entity, CancellationToken.None);
            Context.ChangeTracker.Entries<ExampleEntity>().Where(e => e.State == EntityState.Added).Must().HaveCount(1);
        }
    }

    public class UpdateAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_SetEntityAsModified()
        {
            var entity = await Context.Examples.SingleAsync(e => e.Id == "099");
            var sut = CreateSut(Context);
            _ = await sut.UpdateAsync(entity, CancellationToken.None);
            Context.ChangeTracker.Entries<ExampleEntity>().Where(e => e.State == EntityState.Modified).Must().HaveCount(1);
        }
    }

    public class DeleteAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_SetEntityAsDeleted()
        {
            var entity = await Context.Examples.SingleAsync(e => e.Id == "042");
            var sut = CreateSut(Context);
            _ = await sut.DeleteAsync(entity, CancellationToken.None);
            Context.ChangeTracker.Entries<ExampleEntity>().Where(e => e.State == EntityState.Deleted).Must().HaveCount(1);
        }
    }

    /*
     * Private methods
     */

    private static ExamplesRepository CreateSut(ApiDbContext context) => new(context);
}