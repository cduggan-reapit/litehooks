using FluentValidation;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Extensions;
using Reapit.Platform.CQRS;
using Reapit.Platform.Internal.Common.Database.Models;
using Reapit.Platform.LiteHooks.Core.Exceptions;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;

/// <summary>Handler for the <see cref="GetExamplesQuery"/> request.</summary>
/// <param name="repository">The read-only repository.</param>
/// <param name="validator">The request validator.</param>
public class GetExamplesQueryHandler(
    IReadOnlyExamplesRepository repository,
    IValidator<GetExamplesQuery> validator,
    ILogger<GetExamplesQueryHandler> logger)
    : IRequestHandler<GetExamplesQuery, IEnumerable<ExampleEntity>>
{
    /// <summary>Handles a request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    /// <exception cref="QueryValidationException">when the query parameters did not pass all validation checks.</exception>
    public async Task<IEnumerable<ExampleEntity>> HandleAsync(GetExamplesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Querying {type}: {request}", nameof(ExampleEntity), request.Serialize());
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new QueryValidationException(validation.Errors);

        var dateFilter = new DateFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var entities = await repository.GetAsync(request.Name, request.Description, dateFilter, pagination, cancellationToken);
        return entities;
    }
}