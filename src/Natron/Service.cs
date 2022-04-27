using Microsoft.Extensions.Logging;
using Natron.Config;
using ValidDotNet;

namespace Natron;

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
    }

    public async Task RunAsync()
    {
        try
        {
            _logger.LogInformation("service {_config.Name} started", _config.Name);

            var tasks = _config.Components
                .Select(component => component.RunAsync(_config.CancellationTokenSource.Token))
                .ToArray();

            _logger.LogInformation("{_config.Components.Count} component(s) started", _config.Components.Count);

            await Task.WhenAny(tasks);

            GracefulShutdownComponents();

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "exception caught, gracefully shutting down components");
            GracefulShutdownComponents();
        }
    }

    private void SetupCancelKeyPress()
    {
        Console.CancelKeyPress += (_, ev) =>
        {
            _logger.LogInformation("ctrl+c pressed");
            _cancelKeyPressed = true;
            _config.CancellationTokenSource.Cancel();
            ev.Cancel = true;
        };
    }

    private void GracefulShutdownComponents()
    {
        if (_cancelKeyPressed) return;

        _logger.LogWarning("component returned unexpected. Canceling all components");
        _config.CancellationTokenSource.Cancel();
    }
}