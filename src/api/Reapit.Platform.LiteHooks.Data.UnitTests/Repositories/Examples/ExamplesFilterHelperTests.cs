using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Examples;

public static class ExamplesFilterHelperTests
{
    public class ApplyNameFilter
    {
        [Fact]
        public void Should_ReturnUnfilteredCollection_WhenValueIsNull()
        {
            string? value = null;
            var collection = SeedData;
            var actual = collection.ApplyNameFilter(value);
            actual.Must().BeSameAs(collection);
        }

        [Fact]
        public void Should_ReturnFilteredCollection_WhenValueProvided()
        {
            // There should be 10 results that contain "ame-02"
            const string value = "ame-02";
            var collection = SeedData;
            var actual = collection.ApplyNameFilter(value);
            actual.Must().HaveCount(10).And.AllSatisfy(item => item.Name.Must().Contain(value));
        }
    }

    public class ApplyDescriptionFilter
    {
        [Fact]
        public void Should_ReturnUnfilteredCollection_WhenValueIsNull()
        {
            string? value = null;
            var collection = SeedData;
            var actual = collection.ApplyDescriptionFilter(value);
            actual.Must().BeSameAs(collection);
        }

        [Fact]
        public void Should_ReturnFilteredCollection_WhenValueProvided()
        {
            // There should be 10 results that contain "on-06"
            const string value = "on-06";
            var collection = SeedData;
            var actual = collection.ApplyDescriptionFilter(value);
            actual.Must().HaveCount(10).And.AllSatisfy(item => item.Description.Must().Contain(value));
        }
    }

    /*
     * Setup and private methods
     */

    private static IQueryable<ExampleEntity> SeedData
        => Enumerable.Range(1, 100)
            .Select(i => new ExampleEntity($"name-{i:D3}", $"description-{i:D3}"))
            .AsQueryable();
}