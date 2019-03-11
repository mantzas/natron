using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Config
{
    public sealed class ServiceConfig
    {
        public string Name { get; }
        public ILoggerFactory LoggerFactory { get; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public List<IComponent> Components { get; }
        public TracingConfig TracingConfig { get; private set; }

        public ServiceConfig(string name, ILoggerFactory loggerFactory)
        {
            Name = name.ThrowIfNullOrWhitespace(nameof(name));
            LoggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
            Components = new List<IComponent>();
            CancellationTokenSource = new CancellationTokenSource();
            TracingConfig = new TracingConfig();
        }

        public void UseCancellationTokenSource(CancellationTokenSource cts)
        {
            CancellationTokenSource = cts.ThrowIfNull(nameof(cts));
        }

        public void UseComponents(params IComponent[] components)
        {
            Components.AddRange(components.ThrowIfNullOrEmpty(nameof(components)));
        }

        public void UseTracingConfig(TracingConfig tracingConfig)
        {
            TracingConfig = tracingConfig.ThrowIfNull(nameof(tracingConfig));
        }
    }
}