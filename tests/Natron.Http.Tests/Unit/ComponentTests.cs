using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Natron.Http.Tests.Unit;

public class ComponentTests
{
    [Fact]
    public async Task Component_RunAsync()
    {
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        var config = new Config();
        config.Routes.Add(Route.TracedGet("/test", context => context.Response.WriteAsync("test", cts.Token)));
        config.Routes.Add(new Route("GET", "/test", context => context.Response.WriteAsync("test", cts.Token), false));
        var cmp = new Component(loggerFactory, config);
        var t = cmp.RunAsync(cts.Token);
        await Task.Delay(10, cts.Token);
        cts.Cancel();
        await Task.WhenAll(t);
        t.Status.Should().Be(TaskStatus.RanToCompletion);
    }
}