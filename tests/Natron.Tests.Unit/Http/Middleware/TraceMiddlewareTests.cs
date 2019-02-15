using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Natron.Http.Middleware;
using Xunit;

namespace Natron.Tests.Unit.Http.Middleware
{
    public class TraceMiddlewareTests
    {
        [Fact]
        public async Task TraceMiddleware_InvokeAsync()
        {
            var testRequest = new TestRequestDelegate();
            var traceMiddleware = new ObservabilityMiddleware(testRequest.Next);
            await traceMiddleware.InvokeAsync(new DefaultHttpContext());
            testRequest.Called.Should().BeTrue();
        }

        private class TestRequestDelegate
        {
            public bool Called { get; private set; }

            public Task Next(HttpContext context)
            {
                Called = true;
                return Task.CompletedTask;
            }
        }
    }
}