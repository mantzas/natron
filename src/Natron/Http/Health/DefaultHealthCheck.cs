using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Natron.Http.Health;

public sealed class DefaultHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}