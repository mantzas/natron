using System;
using System.Linq;
using System.Threading.Tasks;
using Jaeger;
using Microsoft.Extensions.Logging;
using Natron.Config;
using OpenTracing.Util;
using ValidDotNet;

namespace Natron
{
    public sealed class Service
    {
        private readonly ServiceConfig _config;
        private readonly ILogger<Service> _logger;
        private bool _cancelKeyPressed;

        internal Service(ServiceConfig config)
        {
            _config = config.ThrowIfNull(nameof(config));
            _logger = config.LoggerFactory.CreateLogger<Service>();
            SetupCancelKeyPress();
            SetupTracing();
        }

        public async Task RunAsync()
        {
            try
            {
                var tasks = _config.Components
                    .Select(component => component.RunAsync(_config.CancellationTokenSource.Token))
                    .ToArray();

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
                _config.CancellationTokenSource.Cancel();
                _cancelKeyPressed = true;
                ev.Cancel = true;
            };
        }

        private void GracefulShutdownComponents()
        {
            if (_cancelKeyPressed)
            {
                return;
            }

            _logger.LogWarning("Component returned unexpected. Canceling all components.");
            _config.CancellationTokenSource.Cancel();
        }

        private void SetupTracing()
        {
            var cfg = new Configuration(_config.Name, _config.LoggerFactory);
            var repCfg = new Configuration.ReporterConfiguration(_config.LoggerFactory);
            repCfg.WithLogSpans(false);
            repCfg.WithFlushInterval(TimeSpan.FromSeconds(1));
            repCfg.SenderConfig.WithAgentHost(_config.TracingConfig.AgentHost);
            repCfg.SenderConfig.WithAgentPort(_config.TracingConfig.AgentPort);
            cfg.WithReporter(repCfg);
            cfg.WithSampler(new Configuration.SamplerConfiguration(_config.LoggerFactory));
            cfg.SamplerConfig.WithType(_config.TracingConfig.SamplerType);
            cfg.SamplerConfig.WithParam(_config.TracingConfig.SamplerParam);
            //cfg.WithMetricsFactory(null);
            GlobalTracer.Register(cfg.GetTracer());
        }
    }
}