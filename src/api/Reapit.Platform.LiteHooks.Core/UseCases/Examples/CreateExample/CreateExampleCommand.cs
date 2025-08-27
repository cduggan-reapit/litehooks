using Reapit.Platform.CQRS;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;

/// <summary>Request for the creation of a new ExampleEntity.</summary>
/// <param name="Name">The name of the entity to create.</param>
/// <param name="Description">An optional description of the entity to create.</param>
public record CreateExampleCommand(string Name, string? Description) : IRequest<ExampleEntity>;