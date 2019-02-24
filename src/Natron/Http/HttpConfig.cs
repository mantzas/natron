using System.Collections.Generic;
using Natron.Http.Health;
using ValidDotNet;

namespace Natron.Http
{
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

        public void AddHealthChecks(params HealthCheck[] healthChecks)
        {
            HealthChecks.AddRange(healthChecks.ThrowIfNull(nameof(healthChecks)));
        }

        public void AddRoutes(params Route[] routes)
        {
            Routes.AddRange(routes.ThrowIfNull(nameof(routes)));
        }

        public void AddUrls(params string[] urls)
        {
            Urls.AddRange(urls.ThrowIfNull(nameof(urls)));
        }
    }
}