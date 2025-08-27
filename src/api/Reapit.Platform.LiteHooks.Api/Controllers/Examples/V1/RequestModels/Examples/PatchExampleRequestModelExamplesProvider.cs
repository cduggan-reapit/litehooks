using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels.Examples;

/// <summary>Examples provider for the <see cref="PatchExampleRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchExampleRequestModelExamplesProvider : IExamplesProvider<PatchExampleRequestModel>
{
    /// <inheritdoc />
    public PatchExampleRequestModel GetExamples()
        => new(
            Name: "Reapit",
            Description: null);
}