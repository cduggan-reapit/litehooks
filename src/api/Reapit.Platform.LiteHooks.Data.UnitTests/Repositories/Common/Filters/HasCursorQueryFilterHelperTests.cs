using Reapit.Platform.LiteHooks.Data.Repositories.Common.Filters;
using Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Common.Filters;

public static class HasCursorQueryFilterHelperTests
{
    public class ApplyCursor
    {
        [Fact]
        public void Should_ReturnUnfilteredCollection_WhenValueIsNull()
        {
            long? value = null;
            var collection = SeedData;
            var actual = collection.ApplyCursor(value);
            actual.Must().BeSameAs(collection);
        }

        [Fact]
        public void Should_ReturnFilteredCollection_WhenValueProvided()
        {
            const long value = 42L;
            var collection = SeedData;
            var actual = collection.ApplyCursor(value);
            actual.Must().HaveCount(58);

            // The data is [1..100], if we say the last read cursor was 42, then the next set would start from 43
            actual.Min()?.Cursor.Must().Be(43);
        }
    }

    public class ApplyPagination
    {
        [Fact]
        public void Should_ReturnPageOfResults()
        {
            const int pageSize = 7;
            const long cursor = 42L; // Same as ApplyCursor tests, this should yield [43..49]

            // Flip the order to make sure that ApplyCursor is sorting the elements
            var actual = SeedData
                .OrderByDescending(record => record.Cursor)
                .ApplyCursor(cursor)
                .ApplyPagination(pageSize);

            actual.Must().HaveCount(pageSize);
            actual.Min()?.Cursor.Must().Be(43);
            actual.Max()?.Cursor.Must().Be(49);
        }
    }

    /*
     * Setup and private methods
     */

    private sealed class HasCursor(long cursor) : IHasCursor, IComparable<HasCursor>
    {
        public long Cursor { get; } = cursor;

        public int CompareTo(HasCursor? other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            return other is null
                ? 1
                : Cursor.CompareTo(other.Cursor);
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="HasCursor"/> representing dates from 2020-01-01 to 2020-04-09 (inclusive)
    /// </summary>
    private static IQueryable<HasCursor> SeedData
        => Enumerable.Range(1, 100)
            .Select(i => new HasCursor(i))
            .AsQueryable();
}