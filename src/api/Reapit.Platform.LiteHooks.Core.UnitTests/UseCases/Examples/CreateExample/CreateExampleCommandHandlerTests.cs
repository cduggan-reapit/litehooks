using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.CreateExample;

public static class CreateExampleCommandHandlerTests
{
    public class Handle
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IExamplesRepository _repository = Substitute.For<IExamplesRepository>();
        private readonly IValidator<CreateExampleCommand> _validator = Substitute.For<IValidator<CreateExampleCommand>>();

        [Fact]
        public async Task Should_ThrowValidationException_WhenValidationFails()
        {
            _validator.ValidateAsync(Arg.Any<CreateExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult([new ValidationFailure("property", "error")]));

            var command = GetRequest();
            var sut = CreateSut();
            var action = () => sut.HandleAsync(command, CancellationToken.None);
            await action.Must().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Should_CreateEntity_WhenValidationSucceeds()
        {
            _validator.ValidateAsync(Arg.Any<CreateExampleCommand>(), Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());

            var command = GetRequest(description: "description");
            var sut = CreateSut();
            _ = await sut.HandleAsync(command, CancellationToken.None);

            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _repository.Received(1).CreateAsync(Arg.Is<ExampleEntity>(ee => ee.Name == command.Name && ee.Description == command.Description), Arg.Any<CancellationToken>());
        }

        private CreateExampleCommandHandler CreateSut()
        {
            _unitOfWork.Examples.Returns(_repository);
            return new CreateExampleCommandHandler(_unitOfWork, _validator, new NullLogger<CreateExampleCommandHandler>());
        }

        private static CreateExampleCommand GetRequest(string name = "name", string? description = null)
            => new(name, description);
    }
}