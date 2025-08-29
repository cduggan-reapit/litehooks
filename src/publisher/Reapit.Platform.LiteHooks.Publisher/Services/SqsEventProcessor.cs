using Amazon.Lambda.SQSEvents;

namespace Reapit.Platform.LiteHooks.Publisher.Services;

/// <inheritdoc/>
public class SqsEventProcessor(ISqsMessageProcessor sqsMessageProcessor) : ISqsEventProcessor
{
    /// <inheritdoc/>
    public async Task ProcessEventAsync(SQSEvent sqsEvent, CancellationToken cancellationToken)
    {
        foreach (var sqsMessage in sqsEvent.Records)
            await sqsMessageProcessor.ProcessMessageAsync(sqsMessage, cancellationToken);
    }
}