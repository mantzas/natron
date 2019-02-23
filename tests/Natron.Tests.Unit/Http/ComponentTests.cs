using System;
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
        public async Task Component_Run()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var cts = new CancellationTokenSource();
            var cmp = new Component(loggerFactory);
            var t = cmp.RunAsync(cts.Token);
            await Task.Delay(1000, cts.Token);
            cts.Cancel();
            await Task.WhenAll(t);
            t.Status.Should().Be(TaskStatus.RanToCompletion);
        }
    }
}