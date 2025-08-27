using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reapit.Platform.LiteHooks.Data.Context;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Context;

public static class ApiDbContextFactoryTests
{
    private const string ReadOnlyConnectionString = "Filename=:memory:;mode=ReadOnly";
    private const string ReadWriteConnectionString = "Filename=:memory:";

    public class CreateDbContext
    {
        [Fact]
        public void Should_CreateReadOnlyContext_WhenNoParameterProvided()
        {
            var sut = CreateSut();
            var context = sut.CreateDbContext();
            var connectionString = context.Database.GetConnectionString();
            connectionString.Must().Be(ReadOnlyConnectionString);
        }

        [Fact]
        public void Should_CreateReadOnlyContext_WhenContextTypeReadOnly()
        {
            var sut = CreateSut();
            var context = sut.CreateDbContext(ContextType.ReadOnly);
            var connectionString = context.Database.GetConnectionString();
            connectionString.Must().Be(ReadOnlyConnectionString);
        }

        [Fact]
        public void Should_CreateReadWriteContext_WhenContextTypeReadWrite()
        {
            var sut = CreateSut();
            var context = sut.CreateDbContext(ContextType.ReadWrite);
            var connectionString = context.Database.GetConnectionString();
            connectionString.Must().Be(ReadWriteConnectionString);
        }
    }

    private static ApiDbContextFactory CreateSut() => new(GetConfiguration());

    private static IConfiguration GetConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("ConnectionStrings:Reader", ReadOnlyConnectionString),
                    new KeyValuePair<string, string?>("ConnectionStrings:Writer", ReadWriteConnectionString)
                ])
            .Build();
}