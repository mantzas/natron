using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Natron.Http.Health;
using Natron.Http.Middleware;
using Prometheus;
using ValidDotNet;

namespace Natron.Http
{
    internal class Component : IComponent
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private IWebHost _webHost;
        private readonly IEnumerable<HealthCheck> _healthChecks;
        private readonly IEnumerable<Route> _routes;

        public Component(ILoggerFactory loggerFactory, IEnumerable<Route> routes = null,
            IEnumerable<HealthCheck> healthChecks = null)
        {
            _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<Component>();
            _routes = routes ?? new Route[0];
            _healthChecks = healthChecks ?? new List<HealthCheck>
            {
                new HealthCheck("default", new DefaultHealthCheck())
            };
        }

        public Task Run(CancellationToken cancelToken)
        {
            _logger.LogInformation("Http Component started");
            _webHost = CreateWebHost();
            return _webHost.RunAsync(cancelToken);
        }

        private IWebHost CreateWebHost()
        {
            var webHost = WebHost.CreateDefaultBuilder(new string[0])
                .Configure(app =>
                {
                    app.UseRouter(BuildRouter(app));
                    app.UseMetricServer();
                    app.UseHealthChecks("/health");
                    app.Build();
                })
                .ConfigureKestrel(options => { })
                .ConfigureLogging(builder => { })
                .ConfigureAppConfiguration(builder => { })
                .ConfigureServices(collection =>
                {
                    foreach (var healthCheck in _healthChecks)
                    {
                        collection.AddHealthChecks().AddCheck(healthCheck.Name, healthCheck.Instance);
                    }

                    collection.AddSingleton(services => _loggerFactory);
                    collection.AddRouting();
                })
                .Build();

            return webHost;
        }

        private IRouter BuildRouter(IApplicationBuilder app)
        {
            var routerBuilder = new RouteBuilder(app);
            foreach (var route in _routes)
            {
                if (route.Trace)
                {
                    _logger.LogInformation("Adding traced route {0} {1}", route.Verb, route.Template);
                    routerBuilder.MapMiddlewareVerb(route.Verb, route.Template, action =>
                        {
                            action.UseMiddleware<ObservabilityMiddleware>();
                            action.Run(route.Handler);
                        }
                    );
                }
                else
                {
                    _logger.LogInformation("Adding traced route {0} {1}", route.Verb, route.Template);
                    routerBuilder.MapVerb(route.Verb, route.Template, route.Handler);
                }
            }

            return routerBuilder.Build();
        }
    }
}