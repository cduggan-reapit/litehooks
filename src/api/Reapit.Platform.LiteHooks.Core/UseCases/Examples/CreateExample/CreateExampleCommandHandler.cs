using FluentValidation;
using Reapit.Platform.CQRS;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Extensions;
using Reapit.Platform.LiteHooks.Data.Services;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;

/// <summary>Handler for the <see cref="CreateExampleCommand"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
/// <param name="validator">The request validator.</param>
public class CreateExampleCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<CreateExampleCommand> validator,
    ILogger<CreateExampleCommandHandler> logger)
    : IRequestHandler<CreateExampleCommand, ExampleEntity>
{
    /// <summary>Handles a request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    /// <exception cref="ValidationException">when the request did not pass all validation checks.</exception>
    public async Task<ExampleEntity> HandleAsync(CreateExampleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating {type}: {request}", nameof(ExampleEntity), request.Serialize());
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = new ExampleEntity(request.Name, request.Description);
        _ = await unitOfWork.Examples.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Created {type}: {entity}", nameof(ExampleEntity), entity.AsSerializable().Serialize());
        return entity;
    }
}