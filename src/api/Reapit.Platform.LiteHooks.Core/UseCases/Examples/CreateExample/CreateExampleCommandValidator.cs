using FluentValidation;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using static Reapit.Platform.LiteHooks.Core.UseCases.Examples.ValidationMessages;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;

/// <summary>Validator for the <see cref="CreateExampleCommand"/> request.</summary>
public class CreateExampleCommandValidator : AbstractValidator<CreateExampleCommand>
{
    private readonly IReadOnlyExamplesRepository _repository;

    /// <summary>Initializes a new instance of the <see cref="CreateExampleCommandValidator"/> class.</summary>
    /// <param name="repository">The read-only repository for database-backed validation.</param>
    public CreateExampleCommandValidator(IReadOnlyExamplesRepository repository)
    {
        _repository = repository;

        // Name: Required, MaxLength(100), Unique
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage(Required)
            .MaximumLength(100).WithMessage(NameExceedsMaxLength)
            .DependentRules(() =>
            {
                RuleFor(request => request.Name)
                    .MustAsync(IsNameAvailable).WithMessage(Unique);
            });

        // Description: MaxLength(1000)
        RuleFor(request => request.Description)
            .MaximumLength(1000).WithMessage(DescriptionExceedsMaxLength);
    }

    private async Task<bool> IsNameAvailable(string name, CancellationToken cancellationToken)
        => await _repository.GetByNameAsync(name, cancellationToken) is null;
}