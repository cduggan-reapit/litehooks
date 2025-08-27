using AutoMapper;
using Reapit.Platform.Testing.Helpers;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Api.UnitTests.Controllers.Examples.V1;

public static class ExamplesProfileTests
{
    public class ExampleEntityToExampleResponseModel
    {
        [Fact]
        public void Should_MapToTarget_WhenDescriptionPopulated()
        {
            var input = new ExampleEntity("name", "description");
            var expected = new ExampleResponseModel(input.Id, input.Name, input.Description);
            var actual = CreateSut().Map<ExampleResponseModel>(input);
            actual.Must().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_MapToTarget_WhenDescriptionNull()
        {
            var input = new ExampleEntity("name", null);
            var expected = new ExampleResponseModel(input.Id, input.Name, null);
            var actual = CreateSut().Map<ExampleResponseModel>(input);
            actual.Must().BeEquivalentTo(expected);
        }
    }

    public class ExampleEntityToExampleDetailResponseModel
    {
        [Fact]
        public void Should_MapToTarget_WhenDescriptionPopulated()
        {
            // We create it with null description and then update it so that the created + modified values differ, this
            // will allow us to detect mis-mapped properties.
            var input = new ExampleEntity("name", null);
            input.Update(null, "description");

            var expected = new ExampleDetailResponseModel(input.Id, input.Name, input.Description, input.DateCreated, input.DateModified);
            var actual = CreateSut().Map<ExampleDetailResponseModel>(input);
            actual.Must().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_MapToTarget_WhenDescriptionNull()
        {
            var input = new ExampleEntity("name", null);
            var expected = new ExampleDetailResponseModel(input.Id, input.Name, input.Description, input.DateCreated, input.DateModified);
            var actual = CreateSut().Map<ExampleDetailResponseModel>(input);
            actual.Must().BeEquivalentTo(expected);
        }
    }

    public class ExampleEntityCollectionToResultPage
    {
        [Fact]
        public void Should_ReturnEmptyResultPage_WhenCollectionEmpty()
        {
            var input = Array.Empty<ExampleEntity>();
            var expected = new ResultPage<ExampleResponseModel>([], 0, 0);
            var actual = CreateSut().Map<ResultPage<ExampleResponseModel>>(input);
            actual.Must().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_ReturnResultPage_WhenCollectionContainsObjects()
        {
            var input = new[]
            {
                new ExampleEntity("one", null),
                new ExampleEntity("two", "dos"),
                new ExampleEntity("three", null)
            };

            var mapper = CreateSut();
            var data = mapper.Map<IEnumerable<ExampleResponseModel>>(input);
            var cursor = input.MaxBy(i => i.Cursor)!.Cursor;

            var expected = new ResultPage<ExampleResponseModel>(data, input.Length, cursor);
            var actual = mapper.Map<ResultPage<ExampleResponseModel>>(input);
            actual.Must().BeEquivalentTo(expected);
        }
    }

    public class GetExamplesRequestModelToGetExamplesQuery
    {
        [Fact]
        public void Should_ReturnDefaultValues_WhenEmptyModelProvided()
        {
            var input = new GetExamplesRequestModel();
            var expected = new GetExamplesQuery(PageSize: 25);
            var actual = CreateSut().Map<GetExamplesQuery>(input);
            actual.Must().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_ReturnMappedValues_WhenProvided()
        {
            var input = new GetExamplesRequestModel(
                Cursor: 123,
                PageSize: 45,
                Name: "name",
                Description: "description",
                CreatedFrom: DateTime.UnixEpoch.AddDays(1),
                CreatedTo: DateTime.UnixEpoch.AddDays(2),
                ModifiedFrom: DateTime.UnixEpoch.AddDays(3),
                ModifiedTo: DateTime.UnixEpoch.AddDays(4));

            var expected = new GetExamplesQuery(
                input.Cursor,
                input.PageSize!.Value,
                input.Name,
                input.Description,
                input.CreatedFrom,
                input.CreatedTo,
                input.ModifiedFrom,
                input.ModifiedTo);

            var actual = CreateSut().Map<GetExamplesQuery>(input);
            actual.Must().BeEquivalentTo(expected);
        }
    }

    public class CreateExampleRequestModelToCreateExampleCommand
    {
        [Fact]
        public void Should_MapToTarget_WhenDescriptionPopulated()
        {
            var input = new CreateExampleRequestModel("name", "description");
            var expected = new CreateExampleCommand(input.Name, input.Description);
            var actual = CreateSut().Map<CreateExampleCommand>(input);
            actual.Must().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_MapToTarget_WhenDescriptionNull()
        {
            var input = new CreateExampleRequestModel("name", null);
            var expected = new CreateExampleCommand(input.Name, null);
            var actual = CreateSut().Map<CreateExampleCommand>(input);
            actual.Must().BeEquivalentTo(expected);
        }
    }

    private static IMapper CreateSut() => AutoMapperFactory.Create<ExamplesProfile>();
}