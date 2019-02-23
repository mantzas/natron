using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Natron.Http;
using Natron.Http.Health;
using ValidDotNet;

namespace Natron
{
    public class Service
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly List<IComponent> _components;
        private readonly CancellationTokenSource _cancelTokenSource;
        private bool _cancelKeyPressed;

        public Service(ILoggerFactory loggerFactory,
            IEnumerable<Route> routes = null,
            IEnumerable<HealthCheck> healthChecks = null,
            IEnumerable<IComponent> components = null)
        {
            _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<Service>();
            _components = new List<IComponent>();
            _cancelTokenSource = new CancellationTokenSource();
            SetupCancelKeyPress();
            SetupDefaultHttpComponent(routes, healthChecks);
            AppendComponents(components);
        }

        public async Task Run()
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var component in _components)
                {
                    tasks.Add(component.RunAsync(_cancelTokenSource.Token));
                }

                await Task.WhenAny(tasks);

                GracefulShutdownComponents();

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught, gracefully shutting down components.");
                GracefulShutdownComponents();
            }
        }

        private void SetupCancelKeyPress()
        {
            Console.CancelKeyPress += (s, ev) =>
            {
                _logger.LogInformation("Ctrl+C pressed.");
                _cancelTokenSource.Cancel();
                _cancelKeyPressed = true;
                ev.Cancel = true;
            };
        }

        private void SetupDefaultHttpComponent(IEnumerable<Route> routes, IEnumerable<HealthCheck> healthChecks)
        {
            _components.Add(new Component(_loggerFactory, routes, healthChecks));
        }

        private void GracefulShutdownComponents()
        {
            if (_cancelKeyPressed)
            {
                return;
            }

            _logger.LogWarning("Component returned unexpected. Canceling all components.");
            _cancelTokenSource.Cancel();
        }

        private void AppendComponents(IEnumerable<IComponent> components)
        {
            if (components == null)
            {
                return;
            }

            _components.AddRange(components);
        }
    }
}