using Microsoft.Extensions.Diagnostics.HealthChecks;
using ValidDotNet;

namespace Natron.Http.Health;

public sealed class HealthCheck
{
    public HealthCheck(string name, IHealthCheck instance)
    {
        Name = name.ThrowIfNullOrWhitespace();
        Instance = instance.ThrowIfNull();
    }

    public string Name { get; }
    public IHealthCheck Instance { get; }

    public static HealthCheck Default()
    {
        return new HealthCheck("default", new DefaultHealthCheck());
    }

    private class DefaultHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new())
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}