using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Kafka.Producer;

public sealed class Producer<TKey, TValue> : IDisposable
{
    private readonly ILogger _logger;
    private readonly IProducer<TKey, TValue> _producer;

    public Producer(ILoggerFactory loggerFactory, ProducerConfig producerConfig)
    {
        _logger = loggerFactory.ThrowIfNull().CreateLogger<Producer<TKey, TValue>>();
        _producer = new ProducerBuilder<TKey, TValue>(producerConfig.ThrowIfNull()).Build();
    }

    public void Dispose()
    {
        _producer.Dispose();
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        topic.ThrowIfNullOrWhitespace();
        message.ThrowIfNull();
        return _producer.ProduceAsync(topic, message, cancellationToken);
    }
}