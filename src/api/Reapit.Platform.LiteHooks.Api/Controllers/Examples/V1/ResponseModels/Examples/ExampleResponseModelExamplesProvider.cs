using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels.Examples;

/// <summary>Examples provider for the <see cref="ExampleResponseModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ExampleResponseModelExamplesProvider : IExamplesProvider<ExampleResponseModel>
{
    /// <inheritdoc />
    public ExampleResponseModel GetExamples()
        => new(
            Id: "e131b0db-fdff-4088-ba5d-72d838b4a14b",
            Name: "Reapit",
            Description: "Reapit is a technology platform built to power your estate or lettings agency.");
}