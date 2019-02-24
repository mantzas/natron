using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Natron.Http;
using NSubstitute;

namespace Natron.Tests.Unit.Http
{
    public class ComponentTests
    {
        [Fact]
        public async Task Component_RunAsync()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var cts = new CancellationTokenSource();
            var cmp = new Component(loggerFactory, urls: new string[] { "http://0.0.0.0:5002", "http://0.0.0.0:5003" });
            var t = cmp.RunAsync(cts.Token);
            await Task.Delay(10, cts.Token);
            cts.Cancel();
            await Task.WhenAll(t);
            t.Status.Should().Be(TaskStatus.RanToCompletion);
        }
    }
}