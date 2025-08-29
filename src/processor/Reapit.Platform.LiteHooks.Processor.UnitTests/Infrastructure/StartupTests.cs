using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Common.Services;
using Reapit.Platform.LiteHooks.Processor.Infrastructure;
using Reapit.Platform.LiteHooks.Processor.Services;

namespace Reapit.Platform.LiteHooks.Processor.UnitTests.Infrastructure;

public class StartupTests
{
    [Fact]
    public void ConfigureServices_AddsRequiredServices()
    {
        var sut = new ServiceCollection();
        sut.ConfigureServices();

        var provider = sut.BuildServiceProvider();
        
        // Local services
        provider.GetService<ISqsEventProcessor>().Must().NotBeNull();
        provider.GetService<ISqsMessageProcessor>().Must().NotBeNull();
        
        // Reapit.Platform.Common
        provider.GetService<IEnvironmentVariableAccessor>().Must().NotBeNull();
    }
}