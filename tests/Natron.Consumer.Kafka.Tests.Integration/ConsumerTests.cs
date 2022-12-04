using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Natron.Consumer.Kafka.Tests.Integration;

[Trait("Category", "Integration")]
public class ConsumerTests
{
    [Fact]
    public async Task TestConsumer()
    {
        var cts = new CancellationTokenSource();

        const string topic = "test-topic";

        var message = new Message<string, string>
        {
            Key = "Key",
            Value = "Value"
        };

        await ProduceMessageAsync(topic, message, cts.Token);

        var lf = Substitute.For<ILoggerFactory>();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "test_client",
            GroupId = "Group1",
            IsolationLevel = IsolationLevel.ReadCommitted,
            
        };
        var topics = new List<string> { topic };

        Message<string, string> expectedMessage = null;

        Task ProcessFunc(Message<string, string> message)
        {
            expectedMessage = message;
            cts.Cancel();
            return Task.FromResult(0);
        }

        var consumer = new Consumer<string, string>(lf, consumerConfig, topics, ProcessFunc);
        await consumer.RunAsync(cts.Token);

        expectedMessage.Should().NotBeNull();
        expectedMessage.Key.Should().Be(message.Key);
        expectedMessage.Value.Should().Be(message.Value);
    }

    private static async Task ProduceMessageAsync(string topic, Message<string, string> message,
        CancellationToken cancellationToken)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "test_producer_client"
        };

        using var producer = new ProducerBuilder<string, string>(
            producerConfig.AsEnumerable()).Build();

        var result = await producer.ProduceAsync(topic, message, cancellationToken);
        result.Status.Should().Be(PersistenceStatus.Persisted);
    }
}