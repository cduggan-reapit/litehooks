using Reapit.Platform.LiteHooks.Data.Repositories.Common.Filters;
using Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Common.Filters;

public static class HasModifiedTimestampQueryFilterHelperTests
{
    public class ApplyModifiedFromFilter
    {
        [Fact]
        public void Should_ReturnUnfilteredCollection_WhenValueIsNull()
        {
            DateTime? value = null;
            var collection = SeedData;
            var actual = collection.ApplyModifiedFromFilter(value);
            actual.Must().BeSameAs(collection);
        }

        [Fact]
        public void Should_ReturnFilteredCollection_WhenValueProvided()
        {
            // Our seed data starts at 2020-01-01, this sets the value as 2020-01-31
            DateTime? value = BaseDate.AddDays(30);
            var collection = SeedData;

            // The result should be applied as >= value
            var expected = collection.Where(e => e.DateModified >= value);
            var actual = collection.ApplyModifiedFromFilter(value);
            actual.Must().HaveCount(70).And.BeEquivalentTo(expected);

            // The result should return records from 2020-01-31 to max (2020-04-09 12:00:00)
            var dates = actual.Select(a => a.DateModified).ToList();
            dates.Max().Must().Be(MaxSeedDate);
            dates.Min().Must().Be(value);
        }
    }

    public class ApplyModifiedToFilter
    {
        [Fact]
        public void Should_ReturnUnfilteredCollection_WhenValueIsNull()
        {
            DateTime? value = null;
            var collection = SeedData;
            var actual = collection.ApplyModifiedToFilter(value);
            actual.Must().BeSameAs(collection);
        }

        [Fact]
        public void Should_ReturnFilteredCollection_WhenValueProvided()
        {
            // Our seed data starts at 2020-01-01, this sets the value as 2020-01-31
            DateTime? value = BaseDate.AddDays(30);
            var collection = SeedData;

            // The result should be applied as < value
            var expected = collection.Where(e => e.DateModified < value);
            var actual = collection.ApplyModifiedToFilter(value);
            actual.Must().HaveCount(30).And.BeEquivalentTo(expected);

            // The result should return records from 2020-01-01 to value-1
            var dates = actual.Select(a => a.DateModified).ToList();
            dates.Max().Must().BeBefore(value.Value);
            dates.Min().Must().Be(BaseDate);
        }
    }

    /*
     * Setup and private methods
     */

    private static readonly DateTime BaseDate = new(2020, 1, 1, 12, 0, 0);
    private static readonly DateTime MaxSeedDate = BaseDate.AddDays(99);

    private sealed class HasModified(DateTime date) : IHasModifiedTimestamp
    {
        public DateTime DateModified { get; } = date;
    }

    /// <summary>
    /// Gets a collection of <see cref="HasModified"/> representing dates from 2020-01-01 to 2020-04-09 (inclusive)
    /// </summary>
    private static IQueryable<HasModified> SeedData
        => Enumerable.Range(0, 100)
            .Select(i => new HasModified(BaseDate.AddDays(i)))
            .AsQueryable();
}