using Reapit.Platform.Internal.Common.Pagination;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels.Examples;

/// <summary>Examples provider for a <see cref="ResultPage{TModel}"/> containing <see cref="ExampleResponseModel"/> objects.</summary>
[ExcludeFromCodeCoverage]
public class ExampleResponseModelResultPageExamplesProvider : IExamplesProvider<ResultPage<ExampleResponseModel>>
{
    /// <inheritdoc />
    public ResultPage<ExampleResponseModel> GetExamples()
        => new(
            Data:
            [
                new ExampleResponseModel(
                    Id: "e131b0db-fdff-4088-ba5d-72d838b4a14b",
                    Name: "Reapit",
                    Description: "Reapit is a technology platform built to power your estate or lettings agency.")
            ],
            Count: 1,
            Cursor: 1_742_459_178_642_136);
}