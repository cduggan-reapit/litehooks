using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.LiteHooks.Publisher.Services;
using Reapit.Platform.LiteHooks.Publisher.Infrastructure;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Reapit.Platform.LiteHooks.Publisher;

/// <summary>The lambda function class.</summary>
public class Function(IServiceCollection services)
{
    private readonly IServiceProvider _serviceProvider = services.BuildServiceProvider();

    /// <summary>Initializes a new instance of the <see cref="Function"/> class.</summary>
    /// <remarks>The parameterless ctor is the live entrypoint from AWS.</remarks>
    public Function() : this(new ServiceCollection().ConfigureServices())
    {
    }

    /// <summary>The lambda function.</summary>
    /// <param name="sqsEvent">The SQS event, containing up to 10 messages.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    public async Task FunctionHandlerAsync(SQSEvent sqsEvent, ILambdaContext context)
    {
        try
        {
            // Create an execution scope
            await using var scope = _serviceProvider.CreateAsyncScope();
            
            // Get the entrypoint service (usually ISqsEventProcessor)
            var processor = scope.ServiceProvider.GetRequiredService<ISqsEventProcessor>();
            await processor.ProcessEventAsync(sqsEvent, CancellationToken.None);
        }
        catch(Exception ex)
        {
            context.Logger.LogError($"An error occurred when processing events: {ex.Message}");
            context.Logger.LogError(ex.ToString());
        }
    }
}
