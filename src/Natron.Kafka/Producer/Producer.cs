using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Kafka.Producer;

public class Producer<TKey, TValue> : IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;

    public Producer(ILoggerFactory loggerFactory, ProducerConfig producerConfig)
    {
        loggerFactory.ThrowIfNull();
        _producer = new ProducerBuilder<TKey, TValue>(producerConfig).Build();
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        return _producer.ProduceAsync(topic, message, cancellationToken);
    }
}