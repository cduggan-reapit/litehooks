using Microsoft.Extensions.Logging.Abstractions;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExampleById;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.GetExampleById;

public static class GetExampleByIdCommandHandlerTests
{
    public class Handle
    {
        private readonly IReadOnlyExamplesRepository _repository = Substitute.For<IReadOnlyExamplesRepository>();

        [Fact]
        public async Task Should_ThrowNotFound_WhenEntityDoesNotExist()
        {
            _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<ExampleEntity?>(null));

            var request = GetRequest();
            var sut = CreateSut();
            var action = () => sut.HandleAsync(request, CancellationToken.None);
            await action.Must().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Should_DeleteEntity_WhenEntityExists()
        {
            var entity = new ExampleEntity("name", "description");
            _repository.GetByIdAsync(entity.Id, Arg.Any<CancellationToken>())
                .Returns(entity);

            var command = GetRequest(entity.Id);
            var sut = CreateSut();
            var actual = await sut.HandleAsync(command, CancellationToken.None);
            actual.Must().BeSameAs(entity);
        }

        private GetExampleByIdQueryHandler CreateSut()
            => new(_repository, new NullLogger<GetExampleByIdQueryHandler>());

        private static GetExampleByIdQuery GetRequest(string id = "id")
            => new(id);
    }
}