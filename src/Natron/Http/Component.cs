﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Natron.Http.Middleware;
using ValidDotNet;

namespace Natron.Http;

public class Component : IComponent
{
    private readonly HttpConfig _config;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public Component(ILoggerFactory loggerFactory, HttpConfig config)
    {
        _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
        _config = config.ThrowIfNull(nameof(config));
        _logger = loggerFactory.CreateLogger<Component>();
    }

    public Task RunAsync(CancellationToken cancelToken)
    {
        _logger.LogInformation("Http Component started");
        return CreateWebHost().RunAsync(cancelToken);
    }

    private IWebHost CreateWebHost()
    {
        var webHost = WebHost.CreateDefaultBuilder(Array.Empty<string>())
            .Configure(app =>
            {
                app.UseRouter(BuildRouter(app));
                app.UseHealthChecks("/health");
                app.Build();
            })
            //.ConfigureKestrel(options => { options.})
            //.ConfigureLogging(builder => { builder. })
            //.ConfigureAppConfiguration(builder => { })
            .ConfigureServices(collection =>
            {
                foreach (var healthCheck in _config.HealthChecks)
                    collection.AddHealthChecks().AddCheck(healthCheck.Name, healthCheck.Instance);

                collection.AddSingleton(_ => _loggerFactory);
                collection.AddRouting();
            })
            .UseUrls(_config.Urls)
            .UseShutdownTimeout(_config.ShutdownTimeout)
            .UseKestrel()
            .Build();

        return webHost;
    }

    private IRouter BuildRouter(IApplicationBuilder app)
    {
        var routerBuilder = new RouteBuilder(app);
        foreach (var route in _config.Routes)
            if (route.Trace)
            {
                _logger.LogInformation("Adding traced route {RouteVerb} {RouteTemplate}", route.Verb, route.Template);
                routerBuilder.MapMiddlewareVerb(route.Verb, route.Template, action =>
                    {
                        action.UseMiddleware<ObservabilityMiddleware>();
                        action.Run(route.Handler);
                    }
                );
            }
            else
            {
                _logger.LogInformation("Adding route {RouteVerb} {RouteTemplate}", route.Verb, route.Template);
                routerBuilder.MapVerb(route.Verb, route.Template, route.Handler);
            }

        return routerBuilder.Build();
    }
}