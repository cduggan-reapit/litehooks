using MediatR;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExampleById;

/// <summary>Request to retrieve the instance of <see cref="ExampleEntity"/> with a given identifier.</summary>
/// <param name="Id">The unique identifier of the entity.</param>
public record GetExampleByIdQuery(string Id) : IRequest<ExampleEntity>;