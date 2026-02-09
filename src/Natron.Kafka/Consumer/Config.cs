using Confluent.Kafka;
using ValidDotNet;

namespace Natron.Kafka.Consumer;

public sealed class Config
{
    public Config(
        ConsumerConfig consumerConfig,
        IEnumerable<string> topics,
        ProcessingStrategy processingStrategy = ProcessingStrategy.Crash)
    {
        ConsumerConfig = consumerConfig.ThrowIfNull();
        Topics = topics.ThrowIfNull();
        ProcessingStrategy = processingStrategy;
    }

    public ConsumerConfig ConsumerConfig { get; }
    public IEnumerable<string> Topics { get; }
    public ProcessingStrategy ProcessingStrategy { get; }
}
