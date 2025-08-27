using MediatR;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.DeleteExample;

/// <summary>Request to delete an existing ExampleEntity.</summary>
/// <param name="Id">The unique identifier of the entity to delete.</param>
public record DeleteExampleCommand(string Id) : IRequest<ExampleEntity>;