using Amazon.Lambda.SQSEvents;

namespace Reapit.Platform.LiteHooks.Processor.Services;

/// <summary>Processor for SQS messages.</summary>
public interface ISqsMessageProcessor
{
    /// <summary>Process an SQS message.</summary>
    /// <param name="sqsMessage">The SQS message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ProcessMessageAsync(SQSEvent.SQSMessage sqsMessage, CancellationToken cancellationToken);
}