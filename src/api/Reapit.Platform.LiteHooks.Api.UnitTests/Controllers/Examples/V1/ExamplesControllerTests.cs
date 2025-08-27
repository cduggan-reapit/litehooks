using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Testing.Helpers;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.DeleteExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExampleById;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Api.UnitTests.Controllers.Examples.V1;

public static class ExamplesControllerTests
{
    public class GetExamples : ExamplesControllerTestBase
    {
        [Fact]
        public async Task Should_ReturnOk_WithResultPage()
        {
            var request = new GetExamplesRequestModel(
                Cursor: 123,
                PageSize: 45,
                Name: "name",
                Description: "description",
                CreatedFrom: DateTime.UnixEpoch.AddDays(1),
                CreatedTo: DateTime.UnixEpoch.AddDays(2),
                ModifiedFrom: DateTime.UnixEpoch.AddDays(3),
                ModifiedTo: DateTime.UnixEpoch.AddDays(4));

            var expectedRequest = Mapper.Map<GetExamplesQuery>(request);
            var expectedContent = Mapper.Map<ResultPage<ExampleResponseModel>>(SeedData);

            Mediator.Send(expectedRequest, Arg.Any<CancellationToken>())
                .Returns(SeedData);

            var sut = CreateSut();
            var result = await sut.GetExamples(request, CancellationToken.None) as OkObjectResult;
            result.Must().NotBeNull().And.Match<OkObjectResult>(r => r.StatusCode == 200);
            result!.Value.Must().BeEquivalentTo(expectedContent);
        }
    }

    public class CreateExamples : ExamplesControllerTestBase
    {
        [Fact]
        public async Task Should_ReturnCreated_WithResponseModel()
        {
            var request = new CreateExampleRequestModel("name", "description");
            var entity = new ExampleEntity(request.Name, request.Description);

            var expectedCommand = new CreateExampleCommand(request.Name, request.Description);
            var expectedContent = Mapper.Map<ExampleResponseModel>(entity);
            Mediator.Send(expectedCommand, Arg.Any<CancellationToken>())
                .Returns(entity);

            var sut = CreateSut();
            var result = await sut.CreateExample(request, CancellationToken.None) as CreatedAtActionResult;
            result.Must().NotBeNull().And.Match<CreatedAtActionResult>(r => r.StatusCode == 201);
            result!.ActionName.Must().Be(nameof(ExamplesController.GetExampleById));
            result.RouteValues.Must().BeEquivalentTo(new Dictionary<string, string> { { "id", entity.Id } });
            result.Value.Must().BeEquivalentTo(expectedContent);
        }
    }

    public class GetExampleById : ExamplesControllerTestBase
    {
        [Fact]
        public async Task Should_ReturnOk_WithDetailModel()
        {
            const string id = "002";
            var entity = SeedData.Single(item => item.Id == id);
            var expectedQuery = new GetExampleByIdQuery(id);
            var expectedContent = Mapper.Map<ExampleDetailResponseModel>(entity);

            Mediator.Send(expectedQuery, Arg.Any<CancellationToken>())
                .Returns(entity);

            var sut = CreateSut();
            var result = await sut.GetExampleById(id, CancellationToken.None) as OkObjectResult;
            result.Must().NotBeNull().And.Match<OkObjectResult>(r => r.StatusCode == 200);
            result!.Value.Must().BeEquivalentTo(expectedContent);
        }
    }

    public class PatchExample : ExamplesControllerTestBase
    {
        [Fact]
        public async Task Should_ReturnNoContent()
        {
            const string id = "002";
            var request = new PatchExampleRequestModel("new name", "new description");
            var expectedCommand = new PatchExampleCommand(id, request.Name, request.Description);

            var sut = CreateSut();
            var result = await sut.PatchExample(id, request, CancellationToken.None) as NoContentResult;
            result.Must().NotBeNull().And.Match<StatusCodeResult>(r => r.StatusCode == 204);

            await Mediator.Received(1).Send(expectedCommand, Arg.Any<CancellationToken>());
        }
    }

    public class DeleteExample : ExamplesControllerTestBase
    {
        [Fact]
        public async Task Should_ReturnNoContent()
        {
            const string id = "002";
            var expectedCommand = new DeleteExampleCommand(id);

            var sut = CreateSut();
            var result = await sut.DeleteExample(id, CancellationToken.None) as NoContentResult;
            result.Must().NotBeNull().And.Match<StatusCodeResult>(r => r.StatusCode == 204);

            await Mediator.Received(1).Send(expectedCommand, Arg.Any<CancellationToken>());
        }
    }

    public abstract class ExamplesControllerTestBase
    {
        protected readonly ISender Mediator = Substitute.For<ISender>();

        protected readonly IMapper Mapper = AutoMapperFactory.Create<ExamplesProfile>();

        protected ExamplesController CreateSut()
            => new(Mediator, Mapper);

        private static readonly DateTime BaseDate = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        protected static IEnumerable<ExampleEntity> SeedData
            => Enumerable.Range(1, 10)
                .Select(seed =>
                {
                    using var idFix = new IdentifierProviderContext($"{seed:D3}");
                    using var dtFix = new DateTimeOffsetProviderContext(BaseDate.AddDays(seed - 1));
                    return new ExampleEntity($"Name {seed:D3}", $"Description {seed:D3}");
                });
    }
}