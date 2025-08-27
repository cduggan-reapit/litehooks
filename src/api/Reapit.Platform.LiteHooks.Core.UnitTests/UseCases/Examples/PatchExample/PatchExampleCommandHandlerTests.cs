using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Testing.Fluent;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.PatchExample;

public static class PatchExampleCommandHandlerTests
{
    public class Handle
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IExamplesRepository _repository = Substitute.For<IExamplesRepository>();
        private readonly IValidator<PatchExampleCommand> _validator = Substitute.For<IValidator<PatchExampleCommand>>();

        [Fact]
        public async Task Should_ThrowValidationException_WhenValidationFails()
        {
            _validator.ValidateAsync(Arg.Any<PatchExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult([new ValidationFailure("property", "error")]));

            var command = GetRequest();
            var sut = CreateSut();
            var action = () => sut.Handle(command, CancellationToken.None);
            await action.Must().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Should_ThrowNotFoundException_WhenEntityDoesNotExist()
        {
            _validator.ValidateAsync(Arg.Any<PatchExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<ExampleEntity?>(null));

            var command = GetRequest();
            var sut = CreateSut();
            var action = () => sut.Handle(command, CancellationToken.None);
            await action.Must().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Should_NotUpdateEntity_WhenUnchanged()
        {
            var entity = new ExampleEntity("original name", "original description");

            _validator.ValidateAsync(Arg.Any<PatchExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(entity);

            var command = GetRequest(entity.Id);
            var sut = CreateSut();
            _ = await sut.Handle(command, CancellationToken.None);

            // No changes, so no update applied
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
            await _repository.DidNotReceive().UpdateAsync(Arg.Any<ExampleEntity>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateEntity_WhenValuesChanged()
        {
            var entity = new ExampleEntity("original name", "original description");

            _validator.ValidateAsync(Arg.Any<PatchExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(entity);

            var command = GetRequest(entity.Id, "new name");
            var sut = CreateSut();
            _ = await sut.Handle(command, CancellationToken.None);

            // The entity has been modified so the DateModified value will differ from the DateCreated value
            entity.DateModified.Must().NotBe(entity.DateCreated);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _repository.Received(1).UpdateAsync(entity, Arg.Any<CancellationToken>());
        }

        private PatchExampleCommandHandler CreateSut()
        {
            _unitOfWork.Examples.Returns(_repository);
            return new PatchExampleCommandHandler(_unitOfWork, _validator, new NullLogger<PatchExampleCommandHandler>());
        }

        private static PatchExampleCommand GetRequest(string id = "id", string? name = null, string? description = null)
            => new(id, name, description);
    }
}