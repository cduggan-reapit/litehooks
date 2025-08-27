using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Extensions;
using Reapit.Platform.LiteHooks.Data.Repositories.Examples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExampleById;

/// <summary>Handler for the <see cref="GetExampleByIdQuery"/> request.</summary>
/// <param name="repository">The read-only repository.</param>
public class GetExampleByIdQueryHandler(IReadOnlyExamplesRepository repository, ILogger<GetExampleByIdQueryHandler> logger)
    : IRequestHandler<GetExampleByIdQuery, ExampleEntity>
{
    /// <summary>Handles a request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    /// <exception cref="NotFoundException">when no entity was found with the requested identifier.</exception>
    public async Task<ExampleEntity> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting {type}: {request}", nameof(ExampleEntity), request.Serialize());
        return await repository.GetByIdAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException(typeof(ExampleEntity), request.Id);
    }
}