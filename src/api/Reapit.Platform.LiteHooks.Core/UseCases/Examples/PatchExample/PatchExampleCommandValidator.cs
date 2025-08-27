using FluentValidation;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using static Reapit.Platform.LiteHooks.Core.UseCases.Examples.ValidationMessages;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;

/// <summary>Validator for the <see cref="PatchExampleCommand"/> request.</summary>
public class PatchExampleCommandValidator : AbstractValidator<PatchExampleCommand>
{
    private readonly IReadOnlyExamplesRepository _repository;

    /// <summary>Initializes a new instance of the <see cref="PatchExampleCommandValidator"/> class.</summary>
    /// <param name="repository">The read-only repository for database-backed validation.</param>
    public PatchExampleCommandValidator(IReadOnlyExamplesRepository repository)
    {
        _repository = repository;

        RuleFor(request => request.Name)
            .Must(name => name is null || name.Length > 0).WithMessage(Required)
            .MaximumLength(100).WithMessage(NameExceedsMaxLength)
            .DependentRules(() =>
            {
                RuleFor(request => request)
                    .MustAsync(IsNameAvailable)
                    .WithName(nameof(PatchExampleCommand.Name))
                    .WithMessage(Unique);
            });

        RuleFor(request => request.Description)
            .MaximumLength(1000).WithMessage(DescriptionExceedsMaxLength);
    }

    private async Task<bool> IsNameAvailable(PatchExampleCommand command, CancellationToken cancellationToken)
    {
        // If name is unchanged, return true.
        if (command.Name is null)
            return true;

        // If nothing exists with the new name, return true. Otherwise, if the existing record with the same name as the
        // command has the same identifier as the subject of the patch command, return true. 
        var extant = await _repository.GetByNameAsync(command.Name, cancellationToken);
        return extant is null || extant.Id.Equals(command.Id, StringComparison.OrdinalIgnoreCase);
    }
}