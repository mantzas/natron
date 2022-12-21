using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.AWS.Consumer;

public class Message
{
    private readonly CancellationToken _cancellationToken;
    private readonly IAmazonSQS _client;
    private readonly ILogger _logger;
    private readonly string _queueUrl;

    public Message(ILoggerFactory loggerFactory, CancellationToken cancellationToken, IAmazonSQS client,
        string queueUrl, Amazon.SQS.Model.Message message)
    {
        _logger = loggerFactory.ThrowIfNull().CreateLogger<Consumer>();
        _queueUrl = queueUrl.ThrowIfNullOrWhitespace();
        _cancellationToken = cancellationToken.ThrowIfNone();
        _client = client.ThrowIfNull();
        Raw = message.ThrowIfNull();
    }

    public Amazon.SQS.Model.Message Raw { get; }

    public async Task AckAsync()
    {
        var request = new DeleteMessageRequest
        {
            QueueUrl = _queueUrl,
            ReceiptHandle = Raw.ReceiptHandle
        };
        var response = await _client.DeleteMessageAsync(request, _cancellationToken).ConfigureAwait(false);

        // TODO: clarify about the status code
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Failed to delete message {MessageId} with HTTP status {HttpStatusCode}",
                Raw.MessageId, response.HttpStatusCode);
            return;
        }

        _logger.LogDebug("Message {MessageId} deleted with HTTP status {HttpStatusCode}",
            Raw.MessageId, response.HttpStatusCode);
    }

    public Task NackAsync()
    {
        _logger.LogDebug("Message {MessageId} not acked", Raw.MessageId);
        return Task.FromResult(0);
    }
}