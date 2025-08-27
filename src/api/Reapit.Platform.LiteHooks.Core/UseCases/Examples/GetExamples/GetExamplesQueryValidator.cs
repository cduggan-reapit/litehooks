using FluentValidation;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;

/// <summary>Validator for the <see cref="GetExamplesQuery"/> request.</summary>
public class GetExamplesQueryValidator : AbstractValidator<GetExamplesQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetExamplesQueryValidator"/> class.</summary>
    public GetExamplesQueryValidator()
    {
        // PageSize must be between 1 and 100.
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(GenericValidationMessages.PageSizeOutOfRange);
    }
}