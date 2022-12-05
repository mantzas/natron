using Amazon.SQS;
using Microsoft.Extensions.Logging;
using Natron.AWS.Consumer;
using Message = Amazon.SQS.Model.Message;

namespace Natron.AWS.Tests.Unit.SQS.Consumer;

[Trait("Category", "Unit")]
public class BatchTests
{
    [Fact]
    public void TestFrom()
    {
        const string queueUrl = "URL";
        var cts = new CancellationTokenSource();
        var lf = Substitute.For<ILoggerFactory>();
        var client = Substitute.For<IAmazonSQS>();

        var rawMessages = new List<Message>
        {
            new() { MessageId = "1" },
            new() { MessageId = "2" }
        };
        var expectedMessages = new List<AWS.Consumer.Message>
        {
            new(lf, cts.Token, client, queueUrl, rawMessages[0]),
            new(lf, cts.Token, client, queueUrl, rawMessages[1])
        };

        var batch = Batch.From(lf, cts.Token, client, queueUrl, rawMessages);

        batch.Messages.Should().BeEquivalentTo(expectedMessages);
    }
}