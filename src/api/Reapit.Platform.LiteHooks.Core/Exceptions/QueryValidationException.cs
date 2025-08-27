using FluentValidation.Results;

namespace Reapit.Platform.LiteHooks.Core.Exceptions;

/// <summary>Represents a validation failure associated with query string parameters.</summary>
/// <param name="message">A message describing the cause of the exception.</param>
public class QueryValidationException(string message) : Exception(message)
{
    internal const string DefaultMessage = "Validation failed for one or more query parameters.";

    /// <summary>The validation errors.</summary>
    public IEnumerable<ValidationFailure> Errors { get; private set; } = [];

    /// <summary>Creates a new QueryValidationException</summary>
    /// <param name="errors"></param>
    public QueryValidationException(IEnumerable<ValidationFailure> errors) : this(DefaultMessage)
        => Errors = errors;
}