using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Natron.Kafka.Consumer;
using Natron.Kafka.Producer;

namespace Natron.Kafka.Tests.Integration;

[Trait("Category", "Integration")]
public class ConsumerTests
{
    [Fact]
    public async Task TestConsumer()
    {
        var cts = new CancellationTokenSource();
        var loggerFactory = Substitute.For<ILoggerFactory>();

        const string topic = "test-topic";

        var message = new Message<string, string>
        {
            Key = "Key",
            Value = "Value"
        };

        await ProduceMessageAsync(loggerFactory, topic, message, cts.Token);


        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "test_client",
            GroupId = "Group1",
            IsolationLevel = IsolationLevel.ReadCommitted,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        var topics = new List<string> { topic };

        Message<string, string>? expectedMessage = null;

        var consumer = new Consumer<string, string>(loggerFactory, consumerConfig, topics, ProcessFunc);
        await consumer.RunAsync(cts.Token);

        expectedMessage.Should().NotBeNull();
        expectedMessage?.Key.Should().Be(message.Key);
        expectedMessage?.Value.Should().Be(message.Value);
        return;

        Task ProcessFunc(Message<string, string> msg)
        {
            expectedMessage = msg;
            cts.Cancel();
            return Task.FromResult(0);
        }
    }

    private static async Task ProduceMessageAsync(ILoggerFactory lf, string topic, Message<string, string> message,
        CancellationToken cancellationToken)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "test_producer_client"
        };

        using var producer = new Producer<string, string>(lf, producerConfig);

        var result = await producer.ProduceAsync(topic, message, cancellationToken);
        result.Status.Should().Be(PersistenceStatus.Persisted);
    }
}