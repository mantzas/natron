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
        await t;
        t.Status.Should().Be(TaskStatus.RanToCompletion);
    }

    private class TestComponent : IComponent
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) await Task.Delay(10, cancellationToken);
        }
    }
}