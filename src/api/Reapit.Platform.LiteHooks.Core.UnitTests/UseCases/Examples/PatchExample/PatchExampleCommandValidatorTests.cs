using Reapit.Platform.Testing.Fluent;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.PatchExample;

public static class PatchExampleCommandValidatorTests
{
    public class Validator
    {
        private readonly IReadOnlyExamplesRepository _repository = Substitute.For<IReadOnlyExamplesRepository>();

        [Fact]
        public async Task Should_Pass_WhenPropertiesNull()
        {
            var command = GetRequest();
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);
            result.IsValid.Must().BeTrue();
        }

        [Fact]
        public async Task Should_Pass_WhenPropertiesValid()
        {
            var command = GetRequest("id", "valid name", "valid description");
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);
            result.IsValid.Must().BeTrue();
        }

        [Fact]
        public async Task Should_Fail_WhenNameEmpty()
        {
            var command = GetRequest(name: "");
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);

            result.Errors.Must().HaveCount(1).And.Contain(e =>
                e.PropertyName == nameof(PatchExampleCommand.Name) && e.ErrorMessage == ValidationMessages.Required);

            await _repository.DidNotReceive().GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Fail_WhenNameTooLong()
        {
            var command = GetRequest(name: new string('a', 101));
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);

            result.Errors.Must().HaveCount(1).And.Contain(e =>
                e.PropertyName == nameof(PatchExampleCommand.Name) && e.ErrorMessage == ValidationMessages.NameExceedsMaxLength);

            await _repository.DidNotReceive().GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Fail_WhenNameUnavailable()
        {
            // The new name is already assigned to `entity`, and `entity` is not the subject of the patch 
            var entity = new ExampleEntity("new name", null);
            _repository.GetByNameAsync(entity.Name, Arg.Any<CancellationToken>())
                .Returns(entity);

            var command = GetRequest(id: "000", name: entity.Name);
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);

            result.Errors.Must().HaveCount(1).And.Contain(e =>
                e.PropertyName == nameof(PatchExampleCommand.Name) && e.ErrorMessage == ValidationMessages.Unique);
        }

        [Fact]
        public async Task Should_Pass_WhenNameAssignedToSubject()
        {
            // The new name is already assigned to `entity`, but `entity` is the subject of the patch 
            var entity = new ExampleEntity("new name", null);
            _repository.GetByNameAsync(entity.Name, Arg.Any<CancellationToken>())
                .Returns(entity);

            var command = GetRequest(id: entity.Id, name: entity.Name);
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);
            result.IsValid.Must().BeTrue();
        }

        [Fact]
        public async Task Should_Pass_WhenNameNotInUse()
        {
            // Nothing is using the new name 
            _repository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<ExampleEntity?>(null));

            var command = GetRequest(name: "name name name");
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);
            result.IsValid.Must().BeTrue();
        }

        [Fact]
        public async Task Should_Fail_WhenDescriptionTooLong()
        {
            var command = GetRequest(description: new string('a', 1001));
            var sut = CreateSut(_repository);
            var result = await sut.ValidateAsync(command);

            result.Errors.Must().HaveCount(1).And.Contain(e =>
                e.PropertyName == nameof(PatchExampleCommand.Description) && e.ErrorMessage == ValidationMessages.DescriptionExceedsMaxLength);
        }
    }

    private static PatchExampleCommandValidator CreateSut(IReadOnlyExamplesRepository repository)
        => new(repository);

    private static PatchExampleCommand GetRequest(string id = "id", string? name = null, string? description = null)
        => new(id, name, description);
}