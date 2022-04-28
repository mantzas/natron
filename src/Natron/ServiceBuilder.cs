using Microsoft.Extensions.Logging;

namespace Natron;

public sealed class ServiceBuilder
{
    private readonly ServiceConfig _config;

    private ServiceBuilder(string name, ILoggerFactory loggerFactory, CancellationTokenSource cts)
    {
        _config = new ServiceConfig(name, loggerFactory, cts);
    }

    public static ServiceBuilder Create(string name, ILoggerFactory loggerFactory, CancellationTokenSource cts)
    {
        return new ServiceBuilder(name, loggerFactory, cts);
    }

    public ServiceBuilder ConfigureComponents(params IComponent[] components)
    {
        _config.UseComponents(components);
        return this;
    }

    public IService Build()
    {
        return new Service(_config);
    }
}