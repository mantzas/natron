using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Natron.AWS.Consumer;
using Message = Amazon.SQS.Model.Message;

namespace Natron.AWS.Tests.Unit.SQS.Consumer;

[Trait("Category", "Unit")]
public class ConsumerTests
{
    [Fact]
    public async Task TestRun()
    {
        const string queueUrl = "URL";
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();

        var rawMessages = new List<Message>
        {
            new() { MessageId = "1" },
            new() { MessageId = "2" }
        };

        var client = Substitute.For<IAmazonSQS>();

        var failureResponse = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.BadRequest
        };

        var successReturn = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Messages = rawMessages
        };

        client.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(failureResponse), Task.FromResult(successReturn));

        client.GetQueueAttributesAsync(Arg.Any<GetQueueAttributesRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GetQueueAttributesResponse
                { HttpStatusCode = HttpStatusCode.OK }));

        var batch = Batch.From(lf, cts.Token, client, queueUrl, rawMessages);

        Batch expectedBatch = null!;

        var config = new Config(queueUrl, ProcessFunc);

        var consumer = new AWS.Consumer.Consumer(lf, client, config);

        await consumer.RunAsync(cts.Token);

        expectedBatch.Should().BeEquivalentTo(batch);
        return;

        Task ProcessFunc(Batch btc)
        {
            expectedBatch = btc;
            cts.Cancel();
            return Task.FromResult(0);
        }
    }
}