using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Natron.Http.Tests.Unit;

[Trait("Category", "Unit")]
public class ComponentTests
{
    [Fact]
    public async Task Component_RunAsync()
    {
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        var config = new Config();
        config.UseRoutes(
            Route.InstrumentedGet("/test", context => context.Response.WriteAsync("test", cts.Token)),
            new Route("GET", "/test2", context => context.Response.WriteAsync("test", cts.Token), false)
        );
        var cmp = new Component(loggerFactory, config);
        var t = cmp.RunAsync(cts.Token);
        await Task.Delay(10, cts.Token);
        await cts.CancelAsync();
        await t;
        t.Status.Should().Be(TaskStatus.RanToCompletion);
    }
}