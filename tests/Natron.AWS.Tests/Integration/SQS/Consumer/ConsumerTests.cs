using System.Net;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Natron.AWS.Consumer;

namespace Natron.AWS.Tests.Integration.SQS.Consumer;

[Trait("Category", "Integration")]
public class ConsumerTests
{
    [Fact]
    public async Task TestConsumer()
    {
        const string notAcked = "Two";

        var (client, queueUrl) = await CreateClientAndQueueAsync(new[] { "One", notAcked, "Three" });

        var lf = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        var counter = 0;

        async Task ProcessFunc(Batch batch)
        {
            foreach (var message in batch.Messages)
                if (message.Raw.Body == notAcked)
                    await message.NackAsync();
                else
                    await message.AckAsync();

            counter++;
            if (counter == 3) await cts.CancelAsync();
        }

        var config = new Config(queueUrl, ProcessFunc, waitTimeSeconds: 1, visibilityTimeout: 1, statsInterval: 1,
            maxNumberOfMessages: 10);
        var consumer = new AWS.Consumer.Consumer(lf, client, config);
        await consumer.RunAsync(cts.Token);

        await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);

        var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = queueUrl
        }, CancellationToken.None);

        response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        response.Messages.Count.Should().Be(1);
        response.Messages[0].Body.Should().Be(notAcked);

        var purgeResponse = await client.PurgeQueueAsync(new PurgeQueueRequest
        {
            QueueUrl = queueUrl
        }, CancellationToken.None);

        purgeResponse.HttpStatusCode.Should().Be(HttpStatusCode.OK);
    }


    private static async Task<Tuple<IAmazonSQS, string>> CreateClientAndQueueAsync(IEnumerable<string> messageIds)
    {
        var client = new AmazonSQSClient(new AnonymousAWSCredentials(), new AmazonSQSConfig
        {
            ServiceURL = "http://localhost:4566",
        });

        var queueResponse = await client.CreateQueueAsync(new CreateQueueRequest
        {
            QueueName = "test-queue"
        });

        if (queueResponse.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Failed to create queue");

        var response = await client.SendMessageBatchAsync(new SendMessageBatchRequest
        {
            Entries = messageIds.Select(p => new SendMessageBatchRequestEntry(p, p)).ToList(),
            QueueUrl = queueResponse.QueueUrl
        });

        if (response.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Failed to send batch");

        if (response.Successful.Count != 3) throw new Exception("Some batch messages failed");


        return new Tuple<IAmazonSQS, string>(client, queueResponse.QueueUrl);
    }
}