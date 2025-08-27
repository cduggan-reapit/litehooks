using FluentValidation;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Extensions;
using Reapit.Platform.CQRS;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;

/// <summary>Handler for the <see cref="PatchExampleCommand"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
/// <param name="validator">The request validator.</param>
public class PatchExampleCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<PatchExampleCommand> validator,
    ILogger<PatchExampleCommandHandler> logger)
    : IRequestHandler<PatchExampleCommand, ExampleEntity>
{
    /// <summary>Handles a request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    /// <exception cref="ValidationException">when the request did not pass all validation checks.</exception>
    /// <exception cref="NotFoundException">when no entity was found with the requested identifier.</exception>
    public async Task<ExampleEntity> HandleAsync(PatchExampleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Patching {type}: {request}", nameof(ExampleEntity), request.Serialize());
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await unitOfWork.Examples.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(ExampleEntity), request.Id);

        entity.Update(request.Name, request.Description);
        if (!entity.IsDirty)
        {
            logger.LogInformation("Unchanged {type}: {entity}", nameof(ExampleEntity), entity.AsSerializable().Serialize());
            return entity;
        }

        _ = await unitOfWork.Examples.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated {type}: {entity}", nameof(ExampleEntity), entity.AsSerializable().Serialize());
        return entity;
    }
}