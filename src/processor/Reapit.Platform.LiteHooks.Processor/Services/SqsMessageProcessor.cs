using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Logging;
using Reapit.Platform.LiteHooks.Processor.Infrastructure;
using Reapit.Platform.Common.Services;

namespace Reapit.Platform.LiteHooks.Processor.Services;

/// <inheritdoc/>
public class SqsMessageProcessor(
    IEnvironmentVariableAccessor environmentVariableAccessor,
    ILogger<SqsMessageProcessor> logger)
    : ISqsMessageProcessor
{
    /// <inheritdoc/>
    public Task ProcessMessageAsync(SQSEvent.SQSMessage sqsMessage, CancellationToken cancellationToken)
    {
        logger.LogInformation("{messageId} - Processing", sqsMessage.MessageId);
        
        if(sqsMessage.Body == environmentVariableAccessor.Get(EnvironmentVariables.Example))
            logger.LogInformation("{messageId} - {body}", sqsMessage.MessageId, sqsMessage.Body);

        return Task.CompletedTask;
    }
}