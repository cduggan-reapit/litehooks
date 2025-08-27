using Reapit.Platform.LiteHooks.Domain.Entities;
using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;

/// <summary>Detailed representation of an ExampleEntity object.</summary>
/// <param name="Id">The unique identifier of the entity.</param>
/// <param name="Name">The name of the entity.</param>
/// <param name="Description">An optional description of the entity.</param>
/// <param name="DateCreated">The creation timestamp for the entity.</param>
/// <param name="DateModified">The last modification timestamp for the entity.</param>
public record ExampleDetailResponseModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("created")] DateTime DateCreated,
    [property: JsonPropertyName("modified")] DateTime DateModified)
{
    /// <summary>Get an instance of <see cref="ExampleDetailResponseModel"/> from a given <see cref="ExampleEntity"/>. </summary>
    /// <param name="entity">The entity to convert.</param>
    public static ExampleDetailResponseModel FromEntity(ExampleEntity entity)
        => new(
            Id: entity.Id,
            Name: entity.Name,
            Description: entity.Description,
            DateCreated: entity.DateCreated.ToUniversalTime(),
            DateModified: entity.DateModified.ToUniversalTime());
};