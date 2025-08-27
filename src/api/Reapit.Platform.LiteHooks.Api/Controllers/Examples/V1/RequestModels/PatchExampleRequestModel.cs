using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;

/// <summary>Request model describing the properties of an Example to be modified.</summary>
/// <param name="Name">The name of the Example.</param>
/// <param name="Description">An optional description of the Example.</param>
public record PatchExampleRequestModel(
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("description")] string? Description = null);