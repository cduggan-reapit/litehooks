using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;

/// <summary>Request model describing the properties of an Example to be created.</summary>
/// <param name="Name">The name of the Example.</param>
/// <param name="Description">An optional description of the Example.</param>
public record CreateExampleRequestModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description)
{
    /// <summary>Create an internal command representing this request model.</summary>
    public CreateExampleCommand ToCommand()
        => new(Name: Name, Description: Description);
}