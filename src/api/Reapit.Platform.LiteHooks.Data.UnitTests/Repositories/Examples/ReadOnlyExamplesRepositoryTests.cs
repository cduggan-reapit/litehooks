using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Internal.Common.Database.Models;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;

namespace Reapit.Platform.LiteHooks.Data.UnitTests.Repositories.Examples;

public static class ReadOnlyExamplesRepositoryTests
{
    public class GetByIdAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_ReturnNull_WhenRecordNotFound()
        {
            var sut = CreateSut(Context);
            var actual = await sut.GetByIdAsync("000", CancellationToken.None);
            actual.Must().BeNull();
        }

        [Fact]
        public async Task Should_ReturnEntity_WhenRecordFound()
        {
            const string id = "042";
            var expected = await Context.Examples.FindAsync(id);
            var sut = CreateSut(Context);
            var actual = await sut.GetByIdAsync(id, CancellationToken.None);
            actual.Must().NotBeNull().And.BeEquivalentTo(expected);
        }
    }

    public class GetByNameAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_ReturnNull_WhenRecordNotFound()
        {
            var sut = CreateSut(Context);
            var actual = await sut.GetByNameAsync("name-000", CancellationToken.None);
            actual.Must().BeNull();
        }

        [Fact]
        public async Task Should_ReturnEntity_WhenRecordFound()
        {
            const string name = "name-017";
            var expected = await Context.Examples.SingleAsync(item => item.Name == name);
            var sut = CreateSut(Context);
            var actual = await sut.GetByNameAsync(name, CancellationToken.None);
            actual.Must().NotBeNull().And.BeEquivalentTo(expected);
        }
    }

    public class GetAsync : ExampleRepositoryTestsBase
    {
        [Fact]
        public async Task Should_ReturnFirstPage_WhenNoParametersProvided()
        {
            var expected = Context.Examples.OrderBy(e => e.Cursor).Take(25);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync();
            actual.Must().HaveCount(25).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_ReturnFirstPage_WhenPageSizeProvided()
        {
            const int pageSize = 52;
            var expected = Context.Examples.OrderBy(e => e.Cursor).Take(pageSize);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(pagination: new PaginationFilter(PageSize: pageSize));
            actual.Must().HaveCount(pageSize).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_ReturnOffsetPage_WhenCursorProvided()
        {
            var cursor = (await Context.Examples.FindAsync("060"))!.Cursor;
            var expected = Context.Examples.Where(e => e.Cursor > cursor).OrderBy(e => e.Cursor).Take(25);

            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(pagination: new PaginationFilter(Cursor: cursor));
            actual.Must().HaveCount(25).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByName_WhenValueProvided()
        {
            const string name = "me-01";
            var expected = Context.Examples.Where(e => e.Name.Contains(name));
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(name: name);
            actual.Must().HaveCount(10).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByDescription_WhenValueProvided()
        {
            const string description = "ion-05";
            var expected = Context.Examples.Where(e => e.Description != null && e.Description.Contains(description));
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(description: description);

            // Expectation is only 9 because every 10th description is null
            actual.Must().HaveCount(9).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByCreatedFromDate_WhenValueProvided()
        {
            var value = BaseDate.AddDays(30);
            var expected = Context.Examples.Where(e => e.DateCreated >= value).OrderBy(e => e.Cursor).Take(25);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(dateFilter: new DateFilter(CreatedFrom: value));
            actual.Must().HaveCount(25).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByCreatedToDate_WhenValueProvided()
        {
            var value = BaseDate.AddDays(20);
            var expected = Context.Examples.Where(e => e.DateCreated < value);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(dateFilter: new DateFilter(CreatedTo: value));
            actual.Must().HaveCount(20).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByModifiedFromDate_WhenValueProvided()
        {
            var value = BaseDate.AddDays(60);
            var expected = Context.Examples.Where(e => e.DateModified >= value).OrderBy(e => e.Cursor).Take(25);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(dateFilter: new DateFilter(ModifiedFrom: value));
            actual.Must().HaveCount(25).And.BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_FilterByModifiedToDate_WhenValueProvided()
        {
            var value = BaseDate.AddDays(20);
            var expected = Context.Examples.Where(e => e.DateModified < value);
            var sut = CreateSut(Context);
            var actual = await sut.GetAsync(dateFilter: new DateFilter(ModifiedTo: value));
            actual.Must().HaveCount(20).And.BeEquivalentTo(expected);
        }
    }

    /*
     * Private methods
     */

    private static ReadOnlyExamplesRepository CreateSut(ApiDbContext context) => new(context);
}