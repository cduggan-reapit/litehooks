using Reapit.Platform.CQRS;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;

/// <summary>Request to modify an existing ExampleEntity.</summary>
/// <param name="Id">The unique identifier of the entity to modify.</param>
/// <param name="Name">The name of the entity to create.</param>
/// <param name="Description">An optional description of the entity to create.</param>
public record PatchExampleCommand(string Id, string? Name, string? Description) : IRequest<ExampleEntity>;