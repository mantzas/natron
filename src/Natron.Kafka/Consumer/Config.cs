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
        var topicsList = topics.ThrowIfNull().ToList();
        if (topicsList.Count == 0)
        {
            throw new ArgumentException("Topics cannot be empty", nameof(topics));
        }
        foreach (var topic in topicsList)
        {
            topic.ThrowIfNullOrWhitespace();
        }
        Topics = topicsList;
        
        if (!Enum.IsDefined(typeof(ProcessingStrategy), processingStrategy))
        {
            throw new ArgumentException($"Invalid ProcessingStrategy: {processingStrategy}", nameof(processingStrategy));
        }
        ProcessingStrategy = processingStrategy;
    }

    public ConsumerConfig ConsumerConfig { get; }
    public IReadOnlyList<string> Topics { get; }
    public ProcessingStrategy ProcessingStrategy { get; }
}
