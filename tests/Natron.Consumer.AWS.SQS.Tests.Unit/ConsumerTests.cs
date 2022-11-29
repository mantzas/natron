using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Natron.Consumer.AWS.SQS.Tests.Unit;

public class ConsumerTests
{
    [Fact]
    public async Task TestRun()
    {
        const string queueUrl = "URL";
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();

        var rawMessages = new List<Amazon.SQS.Model.Message>
        {
            new() { MessageId = "1" },
            new() { MessageId = "2" }
        };

        var client = Substitute.For<IAmazonSQS>();

        var successReturn = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Messages = rawMessages
        };
        var failureResponse = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.BadRequest
        };

        client.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(failureResponse), Task.FromResult(successReturn));

        client.GetQueueAttributesAsync(Arg.Any<GetQueueAttributesRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GetQueueAttributesResponse
                { HttpStatusCode = HttpStatusCode.OK }));

        var batch = Batch.From(lf, cts.Token, client, queueUrl, rawMessages);

        Batch expectedBatch = null!;

        void ProcessFunc(Batch btc)
        {
            expectedBatch = btc;
            cts.Cancel();
        }

        var config = new Config(queueUrl, ProcessFunc);

        var consumer = new Consumer(lf, client, config);

        await consumer.RunAsync(cts.Token);

        expectedBatch.Should().BeEquivalentTo(batch);
    }
}