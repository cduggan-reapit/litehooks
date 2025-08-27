using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels.Examples;

/// <summary>Examples provider for the <see cref="ExampleDetailResponseModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ExampleDetailResponseModelExamplesProvider : IExamplesProvider<ExampleDetailResponseModel>
{
    /// <inheritdoc />
    public ExampleDetailResponseModel GetExamples()
        => new(
            Id: "e131b0db-fdff-4088-ba5d-72d838b4a14b",
            Name: "Reapit",
            Description: "Reapit is a technology platform built to power your estate or lettings agency.",
            DateCreated: new DateTime(1997, 12, 18, 12, 0, 0, DateTimeKind.Utc),
            DateModified: new DateTime(2025, 3, 20, 8, 29, 13, DateTimeKind.Utc));
}