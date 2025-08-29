using Amazon.Lambda.SQSEvents;

namespace Reapit.Platform.LiteHooks.Processor.Services;

/// <summary>Processor for SQS events.</summary>
public interface ISqsEventProcessor
{
    /// <summary>Process an SQS event.</summary>
    /// <param name="sqsEvent">The SQS event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ProcessEventAsync(SQSEvent sqsEvent, CancellationToken cancellationToken);
}