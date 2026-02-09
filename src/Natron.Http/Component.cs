using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Natron.Http.Middleware;
using ValidDotNet;

namespace Natron.Http;

public class Component : IComponent, IAsyncDisposable
{
    private readonly Config _config;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private IHost? _host;

    public Component(ILoggerFactory loggerFactory, Config config)
    {
        _loggerFactory = loggerFactory.ThrowIfNull();
        _config = config.ThrowIfNull();
        _logger = loggerFactory.CreateLogger<Component>();
    }

    public Task RunAsync(CancellationToken cancelToken)
    {
        _logger.LogInformation("Http Component started");
        _host = CreateHost();
        return _host.RunAsync(cancelToken);
    }

    private IHost CreateHost()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel();
        builder.WebHost.UseUrls(_config.Urls);
        builder.WebHost.UseShutdownTimeout(_config.ShutdownTimeout);

        builder.Services.AddSingleton(_loggerFactory);
        builder.Services.AddHealthChecks();

        foreach (var healthCheck in _config.HealthChecks)
        {
            builder.Services.AddHealthChecks().AddCheck(healthCheck.Name, healthCheck.Instance);
        }

        var app = builder.Build();

        app.MapHealthChecks("/health");

        foreach (var route in _config.Routes)
            if (route.Trace)
            {
                _logger.LogInformation("Adding traced route {RouteVerb} {RouteTemplate}", route.Verb, route.Template);
                var middleware = new ObservabilityMiddleware(route.Handler);
                app.MapMethods(route.Template, [route.Verb], middleware.InvokeAsync);
            }
            else
            {
                _logger.LogInformation("Adding route {RouteVerb} {RouteTemplate}", route.Verb, route.Template);
                app.MapMethods(route.Template, [route.Verb], route.Handler);
            }

        return app;
    }

    public async ValueTask DisposeAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}