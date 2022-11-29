using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Natron.Consumer.AWS.SQS.Tests.Unit;

public class MessageTests
{
    [Fact]
    public async Task TestMessageAck()
    {
        const string queueUrl = "URL";
        var rawMessage = new Amazon.SQS.Model.Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        client.DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.OK }));

        var msg = new Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.AckAsync();

        await client.Received(1).DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestMessageAckFailure()
    {
        const string queueUrl = "URL";
        var rawMessage = new Amazon.SQS.Model.Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        client.DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.ServiceUnavailable }));

        var msg = new Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.AckAsync();

        await client.Received(1).DeleteMessageAsync(Arg.Any<DeleteMessageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestMessageNack()
    {
        const string queueUrl = "URL";
        var rawMessage = new Amazon.SQS.Model.Message();
        var cts = new CancellationTokenSource();

        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();
        var msg = new Message(lf, cts.Token, client, queueUrl, rawMessage);

        msg.Raw.Should().Be(rawMessage);

        await msg.NackAsync();
    }
}