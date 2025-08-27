using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Reapit.Platform.Testing.Fluent;
using Reapit.Platform.LiteHooks.Core.Exceptions;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using Reapit.Platform.LiteHooks.Data.Repositories;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.GetExamples;

public static class GetExamplesQueryHandlerTests
{
    public class Handle
    {
        private readonly IReadOnlyExamplesRepository _repository = Substitute.For<IExamplesRepository>();
        private readonly IValidator<GetExamplesQuery> _validator = Substitute.For<IValidator<GetExamplesQuery>>();

        [Fact]
        public async Task Should_ThrowQueryValidationException_WhenValidationFails()
        {
            _validator.ValidateAsync(Arg.Any<GetExamplesQuery>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult([new ValidationFailure("property", "error")]));

            var query = GetRequest();
            var sut = CreateSut();
            var action = () => sut.Handle(query, CancellationToken.None);
            await action.Must().ThrowAsync<QueryValidationException>();
        }

        [Fact]
        public async Task Should_ReturnPage_WhenValidationSucceeds()
        {
            _validator.ValidateAsync(Arg.Any<GetExamplesQuery>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            var query = GetRequest();
            var pagination = new PaginationFilter(query.Cursor, query.PageSize);
            var dateFilter = new DateFilter(query.CreatedFrom, query.CreatedTo, query.ModifiedFrom, query.ModifiedTo);

            var entities = new[]
            {
                new ExampleEntity("one", null),
                new ExampleEntity("two", null),
                new ExampleEntity("three", null)
            };

            _repository.GetAsync(query.Name, query.Description, dateFilter, pagination, Arg.Any<CancellationToken>())
                .Returns(entities);

            var sut = CreateSut();
            var result = await sut.Handle(query, CancellationToken.None);
            result.Must().BeEquivalentTo(entities);
        }

        private GetExamplesQueryHandler CreateSut() => new(_repository, _validator, new NullLogger<GetExamplesQueryHandler>());

        private static GetExamplesQuery GetRequest()
            => new(
                Cursor: 1742399089123,
                PageSize: 25,
                Name: "name",
                Description: "description",
                CreatedFrom: DateTime.UnixEpoch,
                CreatedTo: DateTime.UnixEpoch.AddDays(1),
                ModifiedFrom: DateTime.UnixEpoch.AddDays(2),
                ModifiedTo: DateTime.UnixEpoch.AddDays(3));
    }
}