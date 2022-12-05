using Natron.Http.Health;
using ValidDotNet;

namespace Natron.Http;

public sealed class Config
{
    public Config()
    {
        HealthChecks = new List<HealthCheck> { HealthCheck.Default() };
        Routes = new List<Route>();
        Urls = new[] { "http://0.0.0.0:50000", "https://0.0.0.0:50001" };
        ShutdownTimeout = TimeSpan.FromSeconds(10);
    }

    public List<HealthCheck> HealthChecks { get; private set; }
    public List<Route> Routes { get; }
    public string[] Urls { get; }
    public TimeSpan ShutdownTimeout { get; }

    public void UseHealthChecks(params HealthCheck[] healthChecks)
    {
        HealthChecks = healthChecks.ThrowIfNull(nameof(healthChecks)).ToList();
    }
}