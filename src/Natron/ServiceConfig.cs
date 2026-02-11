using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron;

public sealed class ServiceConfig
{
    public ServiceConfig(string name, ILoggerFactory loggerFactory, CancellationTokenSource cts)
    {
        Name = name.ThrowIfNullOrWhitespace();
        LoggerFactory = loggerFactory.ThrowIfNull();
        Components = [];
        CancellationTokenSource = cts.ThrowIfNull();
    }

    public string Name { get; }
    public ILoggerFactory LoggerFactory { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public IReadOnlyList<IComponent> Components { get; private set; }

    public void UseComponents(params IComponent[] components)
    {
        Components = components.ThrowIfNullOrEmpty().ToList();
    }
}