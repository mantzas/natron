using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace Natron.AWS.Tests.Unit.SQS.Consumer;

[Trait("Category", "Unit")]
public class MessageTests
{
    [Fact]
    public async Task TestMessageAck()
    {
        const string queueUrl = "URL";
        var rawMessage = new Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        client.DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.OK }));

        var msg = new Natron.AWS.Consumer.Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.AckAsync();

        await client.Received(1).DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestMessageAckFailure()
    {
        const string queueUrl = "URL";
        var rawMessage = new Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        client.DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.ServiceUnavailable }));

        var msg = new Natron.AWS.Consumer.Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.AckAsync();

        await client.Received(1).DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestMessageNack()
    {
        const string queueUrl = "URL";
        var rawMessage = new Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        client.ChangeMessageVisibilityAsync(Arg.Any<ChangeMessageVisibilityRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ChangeMessageVisibilityResponse { HttpStatusCode = HttpStatusCode.OK }));
        
        var msg = new Natron.AWS.Consumer.Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.NackAsync();
        
        await client.Received(1).ChangeMessageVisibilityAsync(Arg.Any<ChangeMessageVisibilityRequest>(), Arg.Any<CancellationToken>());
    }
}