using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Kafka.Consumer;

public sealed class Consumer<TKey, TValue> : IComponent
{
    private readonly Config _config;
    private readonly ILogger _logger;
    private readonly Func<Message<TKey, TValue>, Task> _processFunc;

    public Consumer(ILoggerFactory loggerFactory, Config config,
        Func<Message<TKey, TValue>, Task> processFunc)
    {
        _processFunc = processFunc.ThrowIfNull();
        _config = config.ThrowIfNull();
        _logger = loggerFactory.ThrowIfNull().CreateLogger<Consumer<TKey, TValue>>();
    }


    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<TKey, TValue>(_config.ConsumerConfig).Build();
        consumer.Subscribe(_config.Topics);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(cancellationToken);

                if (result.IsPartitionEOF)
                {
                    _logger.LogDebug("Partition EOF reached");
                    continue;
                }

                _logger.LogDebug("Message received {ResultMessage} with offset {ResultOffset}",
                    result.Message.ToString(), result.Offset);

                try
                {
                    await _processFunc(result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message at offset {Offset}", result.Offset);

                    if (_config.ProcessingStrategy == ProcessingStrategy.Crash)
                    {
                        _logger.LogCritical("ProcessingStrategy is set to Crash. Rethrowing exception");
                        throw;
                    }

                    _logger.LogWarning("ProcessingStrategy is set to LogAndContinue. Continuing to next message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer cancellation requested");
        }
        finally
        {
            consumer.Close();
        }
    }
}
