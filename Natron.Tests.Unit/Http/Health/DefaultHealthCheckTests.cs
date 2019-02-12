using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Natron.Http.Health;
using Xunit;

namespace Natron.Tests.Unit.Http.Health
{
    public class DefaultHealthCheckTests
    {
        [Fact]
        public async Task DefaultHealthCheck_Constructor_Throws_OnNullName()
        {
            var result = await new DefaultHealthCheck().CheckHealthAsync(new HealthCheckContext());
            result.Should().NotBeNull();
            result.Status.Should().Be(HealthStatus.Healthy);
        }
    }
}