using System.Globalization;
using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Prometheus;
using ValidDotNet;

namespace Natron.Consumer.AWS.SQS;

public class Message
{
    private const string MessageStatusAck = "ACK";
    private const string MessageStatusAckFailed = "ACK_FAILED";
    private const string MessageStatusNack = "NACK";
    private const string MessageStatusFetched = "FETCHED";

    private static readonly Counter MessageCounter = Metrics
        .CreateCounter("consumer_sqs_message_counter",
            "AWS SQS message processing counter by queue and state e.g. fetched, ack and nack.", "queue", "state");

    private static readonly Gauge MessageAgeGauge = Metrics
        .CreateGauge("consumer_sqs_message_age_seconds", "AWS SQS messages age in seconds.", "queue");

    private readonly CancellationToken _cancellationToken;
    private readonly IAmazonSQS _client;
    private readonly ILogger _logger;
    private readonly string _queueUrl;

    public Message(ILoggerFactory loggerFactory, CancellationToken cancellationToken, IAmazonSQS client,
        string queueUrl, Amazon.SQS.Model.Message message)
    {
        _logger = loggerFactory.ThrowIfNull(nameof(loggerFactory)).CreateLogger<Consumer>();
        _queueUrl = queueUrl.ThrowIfNullOrWhitespace(nameof(queueUrl));
        _cancellationToken = cancellationToken.ThrowIfNone(nameof(cancellationToken));
        _client = client.ThrowIfNull(nameof(client));
        Raw = message.ThrowIfNull(nameof(message));
        MessageCount(MessageStatusFetched);
        MessageAge();
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
            MessageCount(MessageStatusAckFailed);
            return;
        }

        _logger.LogDebug("Message {MessageId} deleted with HTTP status {HttpStatusCode}",
            Raw.MessageId, response.HttpStatusCode);
        MessageCount(MessageStatusAck);
    }

    public Task NackAsync()
    {
        MessageCount(MessageStatusNack);
        _logger.LogDebug("Message {MessageId} nacked", Raw.MessageId);
        return Task.FromResult(0);
    }

    private void MessageCount(string status)
    {
        MessageCounter.WithLabels(_queueUrl, status).Inc();
    }

    private void MessageAge()
    {
        if (!Raw.Attributes.TryGetValue("SentTimestamp", out var value)) return;

        if (!int.TryParse(value, CultureInfo.InvariantCulture, out var epoch))
            return;

        MessageAgeGauge.WithLabels(_queueUrl)
            .Set(DateTime.UtcNow.Subtract(DateTime.UnixEpoch.AddSeconds(epoch)).Seconds);
    }
}