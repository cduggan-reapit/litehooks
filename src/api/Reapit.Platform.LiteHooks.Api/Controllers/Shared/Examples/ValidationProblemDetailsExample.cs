using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.LiteHooks.Api.ExceptionHandlers;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a validation error.</summary>
[ExcludeFromCodeCoverage]
public class ValidationProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = ValidationExceptionHandler.Type,
            Detail = ValidationExceptionHandler.Detail,
            Title = ValidationExceptionHandler.Title,
            Status = ValidationExceptionHandler.StatusCode,
            Extensions = { { "errors", new Dictionary<string, string[]> { { "propertyName", ["errorMessage"] } } } }
        };
}