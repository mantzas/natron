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

    [Fact]
    public async Task TestRun_CrashStrategy_ThrowsOnProcessingError()
    {
        const string queueUrl = "URL";
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();

        var rawMessages = new List<Message>
        {
            new() { MessageId = "1" }
        };

        var client = Substitute.For<IAmazonSQS>();

        var successReturn = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Messages = rawMessages
        };

        client.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(successReturn));

        var config = new Config(queueUrl, ProcessFunc, processingStrategy: ProcessingStrategy.Crash);

        var consumer = new AWS.Consumer.Consumer(lf, client, config);

        Func<Task> act = () => consumer.RunAsync(cts.Token);
        await act.Should().ThrowAsync<InvalidOperationException>();
        return;

        Task ProcessFunc(Batch _)
        {
            throw new InvalidOperationException("test error");
        }
    }

    [Fact]
    public async Task TestRun_LogAndContinueStrategy_ContinuesOnProcessingError()
    {
        const string queueUrl = "URL";
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();

        var rawMessages = new List<Message>
        {
            new() { MessageId = "1" }
        };

        var client = Substitute.For<IAmazonSQS>();

        var successReturn = new ReceiveMessageResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Messages = rawMessages
        };

        client.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(successReturn));

        var callCount = 0;

        var config = new Config(queueUrl, ProcessFunc, processingStrategy: ProcessingStrategy.LogAndContinue);

        var consumer = new AWS.Consumer.Consumer(lf, client, config);

        await consumer.RunAsync(cts.Token);
        callCount.Should().Be(2);
        return;

        Task ProcessFunc(Batch _)
        {
            callCount++;
            if (callCount == 1)
            {
                throw new InvalidOperationException("test error");
            }

            cts.Cancel();
            return Task.CompletedTask;
        }
    }
}