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
        
        if (Urls.Length == 0)
        {
            throw new ArgumentException("Urls cannot be empty", nameof(urls));
        }
        
        if (ShutdownTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("ShutdownTimeout must be positive", nameof(shutdownTimeout));
        }
    }

    public List<HealthCheck> HealthChecks { get; private set; }
    public List<Route> Routes { get; }
    public string[] Urls { get; }
    public TimeSpan ShutdownTimeout { get; }

    public void UseHealthChecks(params HealthCheck[] healthChecks)
    {
        HealthChecks = healthChecks.ThrowIfNull().ToList();
    }
}
