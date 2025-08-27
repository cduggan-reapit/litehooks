using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.LiteHooks.Data.UnitTests.TestHelpers;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Examples;

/// <summary>Base class responsible for setting up the test environment for EntityExample repository tests.</summary>
public class ExampleRepositoryTestsBase : DatabaseTestBase
{
    public override async ValueTask InitializeAsync()
    {
        await Context.Examples.AddRangeAsync(SeedData);
        _ = await Context.SaveChangesAsync();
    }

    protected static readonly DateTime BaseDate = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static IEnumerable<ExampleEntity> SeedData
        => Enumerable.Range(1, 100)
            .Select(i =>
            {
                var name = $"name-{i:D3}";
                var description = i % 10 == 0 ? null : $"description-{i:D3}";

                using var idFixture = new IdentifierProviderContext($"{i:D3}");
                using var timeFixture = new DateTimeOffsetProviderContext(BaseDate.AddDays(i - 1));

                return new ExampleEntity(name, description);
            });
}