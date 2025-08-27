using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;

/// <summary>Lightweight representation of an ExampleEntity object.</summary>
/// <param name="Id">The unique identifier of the entity.</param>
/// <param name="Name">The name of the entity.</param>
/// <param name="Description">An optional description of the entity.</param>
public record ExampleResponseModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description);