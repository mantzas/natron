using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Kafka.Consumer;

public sealed class Consumer<TKey, TValue> : IComponent
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger _logger;
    private readonly Func<Message<TKey, TValue>, Task> _processFunc;
    private readonly IEnumerable<string> _topics;

    public Consumer(ILoggerFactory loggerFactory, ConsumerConfig consumerConfig, IEnumerable<string> topics,
        Func<Message<TKey, TValue>, Task> processFunc)
    {
        _processFunc = processFunc.ThrowIfNull();
        _consumerConfig = consumerConfig.ThrowIfNull();
        _topics = topics.ThrowIfNull();
        _logger = loggerFactory.CreateLogger<Consumer<TKey, TValue>>();
    }


    public async Task RunAsync(CancellationToken cancelToken)
    {
        using var consumer = new ConsumerBuilder<TKey, TValue>(_consumerConfig).Build();
        consumer.Subscribe(_topics);

        while (!cancelToken.IsCancellationRequested)
        {
            var result = consumer.Consume(cancelToken);

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
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process message");
            }
        }

        consumer.Close();
    }
}