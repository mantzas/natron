using Natron.Http.Health;

namespace Natron.Http;

public sealed class HttpConfig
{
    public List<HealthCheck> HealthChecks { get; }
    public List<Route> Routes { get; }
    public List<string> Urls { get; }

    public HttpConfig()
    {
        HealthChecks = new List<HealthCheck>();
        Routes = new List<Route>();
        Urls = new List<string>();
    }
}