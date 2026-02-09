using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Natron;
using Natron.AWS.Consumer;
using Natron.Http;
using Natron.Kafka.Consumer;
using Natron.Kafka.Producer;
using Serilog;
using Config = Natron.Http.Config;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("creating service");

var cts = new CancellationTokenSource();

var httpComponent = CreateHttpComponent();

var sqsComponent = await CreateSqsConsumerAsync();

var kafkaConsumer = await CreateKafkaConsumer();

using var service = ServiceBuilder.Create("example", loggerFactory, cts)
    .ConfigureComponents(httpComponent, sqsComponent, kafkaConsumer)
    .Build();

await service.RunAsync();

return;

IComponent CreateHttpComponent()
{
    var config = new Config();
    return new Component(loggerFactory, config);
}

async Task<IComponent> CreateSqsConsumerAsync()
{
    var client = new AmazonSQSClient(new AnonymousAWSCredentials(), new AmazonSQSConfig
    {
        ServiceURL = "http://localhost:4566"
    });

    var queueResponse = await client.CreateQueueAsync(new CreateQueueRequest
    {
        QueueName = "example-queue"
    });

    var config = new Natron.AWS.Consumer.Config(queueResponse.QueueUrl, ProcessFunc);


    return new Consumer(loggerFactory, client, config);

    Task ProcessFunc(Batch batch)
    {
        logger.LogInformation("SQS Batch received");
        return Task.FromResult(0);
    }
}

async Task<IComponent> CreateKafkaConsumer()
{
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        ClientId = "example_producer_client"
    };

    var producer = new Producer<string, string>(loggerFactory, producerConfig);

    var message = new Message<string, string>
    {
        Key = "Key",
        Value = "Value"
    };

    var topics = new List<string> { "example-topic" };

    await producer.ProduceAsync(topics.First(), message);

    var consumerConfig = new ConsumerConfig
    {
        BootstrapServers = "localhost:9092",
        ClientId = "example_client",
        GroupId = "Group1",
        IsolationLevel = IsolationLevel.ReadCommitted,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    var kafkaConfig = new Natron.Kafka.Consumer.Config(consumerConfig, topics, ProcessingStrategy.LogAndContinue);

    return new Consumer<string, string>(loggerFactory, kafkaConfig, ProcessFuncAsync);

    Task ProcessFuncAsync(Message<string, string> msg)
    {
        logger.LogInformation("Kafka message received");
        return Task.FromResult(0);
    }
}
