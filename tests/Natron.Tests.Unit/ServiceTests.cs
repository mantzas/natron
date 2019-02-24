using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Natron.Tests.Unit
{
    public class ServiceTests
    {
        [Fact]
        public async Task Service_RunAsync()
        {
            var cts = new CancellationTokenSource();
            var lf = Substitute.For<ILoggerFactory>();
            var s = ServiceBuilder.Create(lf).ConfigureCancellationTokenSource(cts).Build();
            var t = s.RunAsync();
            await Task.Delay(100, cts.Token);
            cts.Cancel();
            await Task.WhenAll(t);
            t.Status.Should().Be(TaskStatus.RanToCompletion);
        }
    }
}