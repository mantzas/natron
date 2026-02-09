using Natron.Http.Health;
using ValidDotNet;

namespace Natron.Http;

public sealed class Config
{
    public Config(string[]? urls = null, TimeSpan? shutdownTimeout = null)
    {
        HealthChecks = [HealthCheck.Default()];
        Routes = [];
        Urls = urls ?? ["http://0.0.0.0:50000", "https://0.0.0.0:50001"];
        ShutdownTimeout = shutdownTimeout ?? TimeSpan.FromSeconds(10);
        
        Urls.ThrowIfNull().ThrowIfNullOrEmpty();
        foreach (var url in Urls)
        {
            url.ThrowIfNullOrWhitespace();
        }
        
        if (ShutdownTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("ShutdownTimeout must be greater than zero", nameof(shutdownTimeout));
        }
    }

    public List<HealthCheck> HealthChecks { get; private set; }
    public List<Route> Routes { get; }
    public string[] Urls { get; }
    public TimeSpan ShutdownTimeout { get; }

    public void UseHealthChecks(params HealthCheck[] healthChecks)
    {
        healthChecks.ThrowIfNull();
        foreach (var healthCheck in healthChecks)
        {
            healthCheck.ThrowIfNull();
        }
        HealthChecks = healthChecks.ToList();
    }
}