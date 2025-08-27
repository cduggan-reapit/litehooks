using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;

/// <summary>Model describing the filters to be applied when retrieving a page of Examples.</summary>
/// <param name="Cursor">The page offset.</param>
/// <param name="PageSize">The page size.</param>
/// <param name="Name">Limit results to records with a name containing this value.</param>
/// <param name="Description">Limit results to records with a description containing this value.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this timestamp.</param>
/// <param name="CreatedTo">Limit results to records created before this timestamp.</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this timestamp.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this timestamp.</param>
public record GetExamplesRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int? PageSize = null,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null)
{
    /// <summary>Create an internal query representing this request model.</summary>
    public GetExamplesQuery ToQuery()
        => new(
            Cursor: Cursor,
            PageSize: PageSize ?? ServiceConstants.DefaultPageSize,
            Name: Name,
            Description: Description,
            CreatedFrom: CreatedFrom,
            CreatedTo: CreatedTo,
            ModifiedFrom: ModifiedFrom,
            ModifiedTo: ModifiedTo);
}