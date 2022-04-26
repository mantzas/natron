using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Natron.Http.Health;
using Xunit;

namespace Natron.Http.Tests.Unit.Health;

public class DefaultHealthCheckTests
{
    [Fact]
    public async Task DefaultHealthCheck_CheckHealthAsync()
    {
        var result = await new DefaultHealthCheck().CheckHealthAsync(new HealthCheckContext());
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Healthy);
    }
}