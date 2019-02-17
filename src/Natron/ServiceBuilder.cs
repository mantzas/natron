using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Natron.Http;
using Natron.Http.Health;
using ValidDotNet;

namespace Natron
{
    public sealed class ServiceBuilder
    {
        private ILoggerFactory _loggerFactory;
        private IEnumerable<Route> _routes;
        private IEnumerable<HealthCheck> _healthChecks;
        private IEnumerable<IComponent> _components;

        private ServiceBuilder()
        {
        }

        public static ServiceBuilder Create(ILoggerFactory loggerFactory)
        {
            return new ServiceBuilder
            {
                _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory))
            };
        }

        public ServiceBuilder ConfigureRoutes(IEnumerable<Route> routes)
        {
            _routes = routes.ThrowIfNull(nameof(routes));
            return this;
        }

        public ServiceBuilder ConfigureHealthChecks(IEnumerable<HealthCheck> healthChecks)
        {
            _healthChecks = healthChecks.ThrowIfNull(nameof(healthChecks));
            return this;
        }

        public ServiceBuilder ConfigureComponents(IEnumerable<IComponent> components)
        {
            _components = components.ThrowIfNull(nameof(components));
            return this;
        }

        public Service Build()
        {
            return new Service(_loggerFactory, _routes, _healthChecks, _components);
        }
    }
}