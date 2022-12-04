using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Consumer.Kafka;

public class Consumer<TKey, TValue> : IComponent
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Func<Message<TKey, TValue>, Task> _processFunc;
    private readonly List<string> _topics;

    public Consumer(ILoggerFactory loggerFactory, ConsumerConfig consumerConfig, List<string> topics,
        Func<Message<TKey, TValue>, Task> processFunc)
    {
        _processFunc = processFunc.ThrowIfNull(nameof(processFunc));
        _consumerConfig = consumerConfig.ThrowIfNull(nameof(consumerConfig));
        _topics = topics.ThrowIfNull(nameof(topics));
        _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
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

            await _processFunc(result.Message);
        }

        consumer.Close();
    }
}