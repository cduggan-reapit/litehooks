using MediatR;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;

/// <summary>Request to retrieve a page of ExampleEntity instances matching the given filters.</summary>
/// <param name="Cursor">The page offset.</param>
/// <param name="PageSize">The page size.</param>
/// <param name="Name">Limit results to records with a name containing this value.</param>
/// <param name="Description">Limit results to records with a description containing this value.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this timestamp.</param>
/// <param name="CreatedTo">Limit results to records created before this timestamp.</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this timestamp.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this timestamp.</param>
public record GetExamplesQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? Name = null,
    string? Description = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null)
    : IRequest<IEnumerable<ExampleEntity>>;