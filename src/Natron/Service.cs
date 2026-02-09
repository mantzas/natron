using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron;

public sealed class Service : IService
{
    private readonly ServiceConfig _config;
    private readonly ILogger<Service> _logger;
    private volatile bool _cancelKeyPressed;

    internal Service(ServiceConfig config)
    {
        _config = config.ThrowIfNull();
        _logger = config.LoggerFactory.CreateLogger<Service>();
    }

    public async Task RunAsync()
    {
        try
        {
            SetupCancelKeyPress();

            _logger.LogInformation("service {ConfigName} started", _config.Name);

            var tasks = _config.Components
                .Select(component => component.RunAsync(_config.CancellationTokenSource.Token))
                .ToArray();

            _logger.LogInformation("{ComponentsCount} component(s) started", _config.Components.Count);

            await Task.WhenAny(tasks);

            GracefulShutdownComponents();

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "exception caught, gracefully shutting down components");
            GracefulShutdownComponents();
            throw;
        }
    }

    private void SetupCancelKeyPress()
    {
        Console.CancelKeyPress += OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        _logger.LogInformation("ctrl+c pressed");
        _cancelKeyPressed = true;
        _config.CancellationTokenSource.Cancel();
        e.Cancel = true;
    }

    private void GracefulShutdownComponents()
    {
        if (_cancelKeyPressed) return;

        _logger.LogWarning("component returned unexpected. Canceling all components");
        _config.CancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        _config.CancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}