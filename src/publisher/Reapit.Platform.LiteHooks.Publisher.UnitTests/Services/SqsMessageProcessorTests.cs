using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Logging.Testing;
using Reapit.Platform.Common.Services;
using Reapit.Platform.LiteHooks.Publisher.Infrastructure;
using Reapit.Platform.LiteHooks.Publisher.Services;

namespace Reapit.Platform.LiteHooks.Publisher.Tests.Services;

public class SqsMessageProcessorTests
{
    private readonly IEnvironmentVariableAccessor _environmentVariableAccessor = Substitute.For<IEnvironmentVariableAccessor>();
    private readonly FakeLogger<SqsMessageProcessor> _logger = new();

    [Fact]
    public async Task ProcessMessageAsync_DoesNotLogSecondMessage_WhenBodyDoesNotMatchEnvironmentVariable()
    {
        const string value = "value";

        _environmentVariableAccessor.Get(EnvironmentVariables.Example)
            .Returns(value);

        var input = new SQSEvent.SQSMessage { MessageId = "message-id", Body = "different" };

        var sut = CreateSut();
        await sut.ProcessMessageAsync(input, CancellationToken.None);

        _logger.LatestRecord.Must().NotBeNull();
    }

    [Fact]
    public async Task ProcessMessageAsync_LogsSecondMessage_WhenBodyMatchesEnvironmentVariable()
    {
        const string value = "value";

        _environmentVariableAccessor.Get(EnvironmentVariables.Example)
            .Returns(value);

        var input = new SQSEvent.SQSMessage { MessageId = "message-id", Body = value };

        var sut = CreateSut();
        await sut.ProcessMessageAsync(input, CancellationToken.None);

        _logger.Collector.Count.Must().Be(2);
    }
    
    /*
     * Private methods
     */

    private SqsMessageProcessor CreateSut()
        => new(_environmentVariableAccessor, _logger);
}