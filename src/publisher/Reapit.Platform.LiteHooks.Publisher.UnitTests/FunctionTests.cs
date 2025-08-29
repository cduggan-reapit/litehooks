using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using Reapit.Platform.LiteHooks.Publisher.Services;

namespace Reapit.Platform.LiteHooks.Publisher.Tests;

public class FunctionTests
{
    private readonly ISqsEventProcessor _eventProcessor = Substitute.For<ISqsEventProcessor>();

    [Fact]
    public async Task FunctionHandler_ProcessesSqsEvent_WhenNoExceptionsThrown()
    {
        var input = new SQSEvent { Records = [new SQSEvent.SQSMessage { MessageId = "01" }] };
        var context = Substitute.For<ILambdaContext>();
        
        var sut = CreateSut();
        await sut.FunctionHandlerAsync(input, context);
        
        context.Logger.DidNotReceive().LogError(Arg.Any<string>());
        await _eventProcessor.Received(1).ProcessEventAsync(Arg.Any<SQSEvent>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task FunctionHandler_LogsErrors_WhenExceptionsThrown()
    {
        var input = new SQSEvent { Records = [new SQSEvent.SQSMessage { MessageId = "01" }] };
        var context = Substitute.For<ILambdaContext>();

        _eventProcessor.ProcessEventAsync(Arg.Any<SQSEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("simulated-exception"));
        
        var sut = CreateSut();
        await sut.FunctionHandlerAsync(input, context);
        
        await _eventProcessor.Received(1).ProcessEventAsync(Arg.Any<SQSEvent>(), Arg.Any<CancellationToken>());
        context.Logger.Received(2).LogError(Arg.Any<string>());
    }
    
    /*
     * Private methods
     */

    private Function CreateSut()
        => new(new ServiceCollection().AddSingleton(_eventProcessor));
}