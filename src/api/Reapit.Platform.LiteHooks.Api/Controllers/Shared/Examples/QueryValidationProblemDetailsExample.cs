using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.LiteHooks.Api.ExceptionHandlers;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a query string validation error.</summary>
[ExcludeFromCodeCoverage]
public class QueryValidationProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = QueryValidationExceptionHandler.Type,
            Detail = "Validation failed for one or more query parameters.",
            Title = QueryValidationExceptionHandler.Title,
            Status = QueryValidationExceptionHandler.StatusCode,
            Extensions = { { "errors", new Dictionary<string, string[]> { { "propertyName", ["errorMessage"] } } } }
        };
}