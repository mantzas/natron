﻿using Amazon.Runtime;
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
using Config = Natron.AWS.Consumer.Config;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog();
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("creating service");

var cts = new CancellationTokenSource();

var httpComponent = CreateHttpComponent();

var sqsComponent = await CreateSqsConsumerAsync();

var kafkaConsumer = await CreateKafkaConsumer();

await ServiceBuilder.Create("example", loggerFactory, cts)
    .ConfigureComponents(httpComponent, sqsComponent, kafkaConsumer)
    .Build().RunAsync();

IComponent CreateHttpComponent()
{
    var config = new HttpConfig();
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

    Task ProcessFunc(Batch batch)
    {
        logger.LogInformation("SQS Batch received");
        return Task.FromResult(0);
    }

    var config = new Config(queueResponse.QueueUrl, ProcessFunc);


    return new Consumer(loggerFactory, client, config);
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

    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9092",
        ClientId = "example_client",
        GroupId = "Group1",
        IsolationLevel = IsolationLevel.ReadCommitted,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };


    Task ProcessFuncAsync(Message<string, string> message)
    {
        logger?.LogInformation("Kafka message received");
        return Task.FromResult(0);
    }

    return new Consumer<string, string>(loggerFactory, config, topics, ProcessFuncAsync);
}