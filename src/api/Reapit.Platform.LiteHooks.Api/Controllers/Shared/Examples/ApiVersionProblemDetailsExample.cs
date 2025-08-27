using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a missing resource.</summary>
[ExcludeFromCodeCoverage]
public class ApiVersionProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://docs.api-versioning.org/problems#unspecified",
            Detail = "An API version is required, but was not specified.",
            Title = "Unspecified API version",
            Status = 400
        };
}