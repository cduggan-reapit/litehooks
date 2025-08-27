using Reapit.Platform.LiteHooks.Domain.Entities;
using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;

/// <summary>Lightweight representation of an ExampleEntity object.</summary>
/// <param name="Id">The unique identifier of the entity.</param>
/// <param name="Name">The name of the entity.</param>
/// <param name="Description">An optional description of the entity.</param>
public record ExampleResponseModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description)
{
    /// <summary>Get an instance of <see cref="ExampleResponseModel"/> from a given <see cref="ExampleEntity"/>. </summary>
    /// <param name="entity">The entity to convert.</param>
    public static ExampleResponseModel FromEntity(ExampleEntity entity)
        => new(Id: entity.Id, Name: entity.Name, Description: entity.Description);
};