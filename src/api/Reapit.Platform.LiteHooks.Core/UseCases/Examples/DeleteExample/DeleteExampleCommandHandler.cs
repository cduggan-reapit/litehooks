using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Extensions;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.DeleteExample;

/// <summary>Handler for the <see cref="DeleteExampleCommand"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
public class DeleteExampleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteExampleCommandHandler> logger)
    : IRequestHandler<DeleteExampleCommand, ExampleEntity>
{
    /// <summary>Handles a request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    /// <exception cref="NotFoundException">when no entity was found with the requested identifier.</exception>
    public async Task<ExampleEntity> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting {type}: {request}", nameof(ExampleEntity), request.Serialize());
        var entity = await unitOfWork.Examples.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(ExampleEntity), request.Id);

        entity.Delete();
        _ = await unitOfWork.Examples.UpdateAsync(entity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deleted {type}: {entity}", nameof(ExampleEntity), entity.AsSerializable().Serialize());
        return entity;
    }
}