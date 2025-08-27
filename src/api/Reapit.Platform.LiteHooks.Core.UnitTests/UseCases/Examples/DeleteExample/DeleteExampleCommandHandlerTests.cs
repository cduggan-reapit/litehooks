using Microsoft.Extensions.Logging.Abstractions;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.DeleteExample;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.DeleteExample;

public static class DeleteExampleCommandHandlerTests
{
    public class Handle
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IExamplesRepository _repository = Substitute.For<IExamplesRepository>();

        [Fact]
        public async Task Should_ThrowNotFound_WhenEntityDoesNotExist()
        {
            _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<ExampleEntity?>(null));

            var command = GetRequest();
            var sut = CreateSut();
            var action = () => sut.HandleAsync(command, CancellationToken.None);
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
            _ = await sut.HandleAsync(command, CancellationToken.None);

            entity.DateDeleted.Must().NotBeNull();
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _repository.Received(1).UpdateAsync(entity, Arg.Any<CancellationToken>());
        }

        private DeleteExampleCommandHandler CreateSut()
        {
            _unitOfWork.Examples.Returns(_repository);
            return new DeleteExampleCommandHandler(_unitOfWork, new NullLogger<DeleteExampleCommandHandler>());
        }

        private static DeleteExampleCommand GetRequest(string id = "id")
            => new(id);
    }
}