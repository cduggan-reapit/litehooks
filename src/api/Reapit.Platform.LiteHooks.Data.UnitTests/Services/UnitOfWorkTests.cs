using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Data.UnitTests.TestHelpers;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Services;

public static class UnitOfWorkTests
{
    public class Examples : DatabaseTestBase
    {
        [Fact]
        public void Should_ReturnsRepository_WhenCalledForTheFirstTime()
        {
            var sut = CreateSut(Context);
            var repository = sut.Examples;
            repository.Must().NotBeNull();
        }

        [Fact]
        public void Should_ReusesRepository_ForSubsequentCalls()
        {
            var sut = CreateSut(Context);
            var firstRepository = sut.Examples;
            var secondRepository = sut.Examples;
            secondRepository.Must().BeSameAs(firstRepository);
        }
    }

    public class SaveChangesAsync : DatabaseTestBase
    {
        [Fact]
        public async Task Should_CommitChanges()
        {
            var sut = CreateSut(Context);
            await sut.Examples.CreateAsync(new ExampleEntity("name", "description"), CancellationToken.None);

            Context.ChangeTracker.Entries().Must().HaveCount(1).And.Contain(entry => entry.State == EntityState.Added);
            await sut.SaveChangesAsync(CancellationToken.None);

            // Added changes to Unchanged once saved
            Context.ChangeTracker.Entries().Must().HaveCount(1).And.Contain(entry => entry.State == EntityState.Unchanged);
            Context.Examples.Must().HaveCount(1);

        }
    }

    public class Dispose : DatabaseTestBase
    {
        [Fact]
        public void Should_DisposeContext_IfInitialized()
        {
            var sut = CreateSut(Context);

            // Call this to make sut initialize the context.
            _ = sut.Examples;

            // Disposing the sut should dispose the initialized context (in this case `Context`)
            sut.Dispose();

            // Trying to work with the context after disposal should throw ObjectDisposedException
            var action = () => Context.SaveChanges();
            action.Must().Throw<ObjectDisposedException>();
        }

        [Fact]
        public void Should_DoNothing_IfContextNotInitialized()
        {
            // This time we won't assign _context, so Context won't be disposed when sut is
            var sut = CreateSut(Context);
            sut.Dispose();

            var action = () => Context.SaveChanges();
            action.Must().NotThrow();
        }
    }

    public class DisposeAsync : DatabaseTestBase
    {
        [Fact]
        public async Task Should_DisposeContext_IfInitialized()
        {
            var sut = CreateSut(Context);

            // Call this to make sut initialize the context
            _ = await sut.Examples.GetAsync();

            // Disposing the sut should dispose the initialized context (in this case `Context`)
            await sut.DisposeAsync();

            // Trying to work with the context after disposal should throw ObjectDisposedException
            var action = () => Context.SaveChangesAsync();
            await action.Must().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        public async Task Should_DoNothing_IfContextNotInitialized()
        {
            // This time we won't assign _context, so Context won't be disposed when sut is
            var sut = CreateSut(Context);
            await sut.DisposeAsync();

            var action = () => Context.SaveChangesAsync();
            await action.Must().NotThrowAsync();
        }
    }

    private static UnitOfWork CreateSut(ApiDbContext context)
        => new(GetDbContextFactory(context));

    private static IApiDbContextFactory GetDbContextFactory(ApiDbContext context)
    {
        var factory = Substitute.For<IApiDbContextFactory>();
        factory.CreateDbContext(ContextType.ReadWrite).Returns(context);
        return factory;
    }
}