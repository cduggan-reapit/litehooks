using Amazon.Lambda.SQSEvents;
using Reapit.Platform.LiteHooks.Processor.Services;

namespace Reapit.Platform.LiteHooks.Processor.UnitTests.Services;

public class SqsEventProcessorTests
{
    private readonly ISqsMessageProcessor _sqsMessageProcessor = Substitute.For<ISqsMessageProcessor>();

    [Fact]
    public async Task ProcessEventAsync_CallsMessageProcessor_ForEachMessage()
    {
        var sqsMessages = GetMessages(10);
        var sqsEvent = new SQSEvent { Records = sqsMessages.ToList() };

        var sut = CreateSut();
        await sut.ProcessEventAsync(sqsEvent, CancellationToken.None);
        
        await _sqsMessageProcessor.Received(10).ProcessMessageAsync(Arg.Any<SQSEvent.SQSMessage>(), Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private SqsEventProcessor CreateSut()
        => new(_sqsMessageProcessor);

    private static IEnumerable<SQSEvent.SQSMessage> GetMessages(int count)
        => Enumerable.Range(1, count)
            .Select(i => new SQSEvent.SQSMessage
            {
                MessageId = $"{i:D3}", 
                Body = $"Message {i:D3}"
            });
}