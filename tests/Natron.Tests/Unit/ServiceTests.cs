using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Natron.Tests.Unit;

[Trait("Category", "Unit")]
public class ServiceTests
{
    [Fact]
    public async Task Service_RunAsync()
    {
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();
        var cmp = new TestComponent();
        var s = ServiceBuilder.Create("test", lf, cts)
            .ConfigureComponents(cmp)
            .Build();
        var t = s.RunAsync();
        await Task.Delay(100, cts.Token);
        await cts.CancelAsync();
        
        // Expect TaskCanceledException to be thrown since we now re-throw exceptions
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await t);
    }

    private class TestComponent : IComponent
    {
        public async Task RunAsync(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested) await Task.Delay(10, cancelToken);
        }
    }
}