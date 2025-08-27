using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels.Examples;

/// <summary>Examples provider for the <see cref="CreateExampleRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateExampleRequestModelExamplesProvider : IExamplesProvider<CreateExampleRequestModel>
{
    /// <inheritdoc />
    public CreateExampleRequestModel GetExamples()
        => new(
            Name: "Reapit",
            Description: "Reapit is a technology platform built to power your estate or lettings agency.");
}