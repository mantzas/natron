using Amazon.SQS;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Consumer.AWS.SQS;

public class Batch
{
    private Batch(List<Message> messages)
    {
        Messages = messages.ThrowIfNull(nameof(messages));
    }

    public List<Message> Messages { get; }

    public static Batch From(ILoggerFactory loggerFactory, CancellationToken cancellationToken, IAmazonSQS client,
        string queueUrl, IEnumerable<Amazon.SQS.Model.Message> rawMessages)
    {
        var messages = rawMessages.Select(x => new Message(loggerFactory, cancellationToken, client, queueUrl, x))
            .ToList();

        return new Batch(messages);
    }
}