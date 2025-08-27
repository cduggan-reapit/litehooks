using Reapit.Platform.Testing.Fluent;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.CreateExample;

public static class CreateExampleCommandValidatorTests
{
    public class Validator
    {
        private readonly IReadOnlyExamplesRepository _repository = Substitute.For<IReadOnlyExamplesRepository>();

        [Fact]
        public async Task Should_Pass_WhenRequestValid()
        {
            _repository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<ExampleEntity?>(null));

            var command = GetRequest();
            var sut = CreateSut(_repository);
            var actual = await sut.ValidateAsync(command, CancellationToken.None);
            actual.IsValid.Must().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_Fail_WhenNameNotProvided(string? name)
        {
            var command = GetRequest(name: name!);
            var sut = CreateSut(_repository);
            var actual = await sut.ValidateAsync(command, CancellationToken.None);

            actual.Errors.Must().HaveCount(1).And.Contain(failure =>
                failure.PropertyName == nameof(CreateExampleCommand.Name)
                && failure.ErrorMessage == ValidationMessages.Required);

            // Doesn't hit the database if the simple name validation fails
            await _repository.DidNotReceive().GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Fail_WhenNameTooLong()
        {
            var command = GetRequest(name: new string('a', 101));
            var sut = CreateSut(_repository);
            var actual = await sut.ValidateAsync(command, CancellationToken.None);

            actual.Errors.Must().HaveCount(1).And.Contain(failure =>
                failure.PropertyName == nameof(CreateExampleCommand.Name)
                && failure.ErrorMessage == ValidationMessages.NameExceedsMaxLength);

            // Doesn't hit the database if the simple name validation fails
            await _repository.DidNotReceive().GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Fail_WhenNameInUse()
        {
            const string newName = "new name";

            _repository.GetByNameAsync(newName, Arg.Any<CancellationToken>())
                .Returns(new ExampleEntity(newName, null));

            var command = GetRequest(name: newName);
            var sut = CreateSut(_repository);
            var actual = await sut.ValidateAsync(command, CancellationToken.None);

            actual.Errors.Must().HaveCount(1).And.Contain(failure =>
                failure.PropertyName == nameof(CreateExampleCommand.Name)
                && failure.ErrorMessage == ValidationMessages.Unique);
        }

        [Fact]
        public async Task Should_Fail_WhenDescriptionTooLong()
        {
            var command = GetRequest(description: new string('a', 1001));
            var sut = CreateSut(_repository);
            var actual = await sut.ValidateAsync(command, CancellationToken.None);

            actual.Errors.Must().HaveCount(1).And.Contain(failure =>
                failure.PropertyName == nameof(CreateExampleCommand.Description)
                && failure.ErrorMessage == ValidationMessages.DescriptionExceedsMaxLength);
        }
    }

    private static CreateExampleCommandValidator CreateSut(IReadOnlyExamplesRepository repository)
        => new(repository);

    private static CreateExampleCommand GetRequest(string name = "valid name", string? description = null)
        => new(name, description);
}