using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron;

public sealed class Service : IService
{
    private readonly ServiceConfig _config;
    private readonly ILogger<Service> _logger;
    private bool _cancelKeyPressed;
    private int _running;

    internal Service(ServiceConfig config)
    {
        _config = config.ThrowIfNull();
        _logger = config.LoggerFactory.CreateLogger<Service>();
    }

    public async Task RunAsync()
    {
        if (Interlocked.CompareExchange(ref _running, 1, 0) != 0)
        {
            throw new InvalidOperationException("Service is already running. RunAsync cannot be called multiple times.");
        }

        Task[] tasks = [];

        try
        {
            SetupCancelKeyPress();

            _logger.LogInformation("service {ConfigName} started", _config.Name);

            tasks = _config.Components
                .Select(component => component.RunAsync(_config.CancellationTokenSource.Token))
                .ToArray();

            _logger.LogInformation("{ComponentsCount} component(s) started", _config.Components.Count);

            await Task.WhenAny(tasks);

            GracefulShutdownComponents();

            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("service {ConfigName} cancellation requested", _config.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "exception caught, gracefully shutting down components");
            _config.CancellationTokenSource.Cancel();
            await Task.WhenAll(tasks);
        }
        finally
        {
            await DisposeComponentsAsync();
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

    private async Task DisposeComponentsAsync()
    {
        foreach (var component in _config.Components)
        {
            try
            {
                switch (component)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error disposing component {ComponentType}", component.GetType().Name);
            }
        }
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
    }
}