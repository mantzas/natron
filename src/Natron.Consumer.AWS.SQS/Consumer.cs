using System.Net;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.Consumer.AWS.SQS;

public class Consumer : IComponent
{
    private readonly Config _config;
    private readonly AWSCredentials _credentials;
    private readonly ILogger _logger;
    private readonly AmazonSQSConfig _sqsConfig;

    public Consumer(ILoggerFactory loggerFactory, Config config)
    {
        _config = config.ThrowIfNull(nameof(config));
        _logger = loggerFactory.ThrowIfNull(nameof(loggerFactory)).CreateLogger<Consumer>();
        _credentials = new AnonymousAWSCredentials();
        _sqsConfig = new AmazonSQSConfig();
    }

    public async Task RunAsync(CancellationToken cancelToken)
    {
        var sqsClient = new AmazonSQSClient(_credentials, _sqsConfig);

        while (!cancelToken.IsCancellationRequested)
        {
            var messages = await GetMessages(sqsClient, cancelToken);

            foreach (var message in messages)
            {
                if (cancelToken.IsCancellationRequested) return;

                _config.ProcessFunc(message);

                await DeleteMessage(sqsClient, cancelToken, message);
            }
        }
    }

    private async Task<List<Message>> GetMessages(IAmazonSQS sqsClient, CancellationToken cancelToken)
    {
        var request = new ReceiveMessageRequest
        {
            MessageAttributeNames = new List<string> { "All" },
            AttributeNames = new List<string> { "All" },
            QueueUrl = _config.QueueUrl,
            MaxNumberOfMessages = _config.MaxNumberOfMessages,
            VisibilityTimeout = _config.VisibilityTimeout,
            WaitTimeSeconds = _config.WaitTimeSeconds
        };

        var response = await sqsClient.ReceiveMessageAsync(request, cancelToken).ConfigureAwait(false);

        // TODO: clarify about the status code
        if (response.HttpStatusCode == HttpStatusCode.OK) return response.Messages;

        _logger.LogError("Failed to receive messages with HTTP status code {HttpStatusCode}",
            response.HttpStatusCode);
        return new List<Message>();
    }

    private async Task DeleteMessage(IAmazonSQS sqsClient, CancellationToken cancelToken, Message msg)
    {
        var request = new DeleteMessageRequest
        {
            QueueUrl = _config.QueueUrl,
            ReceiptHandle = msg.ReceiptHandle
        };

        try
        {
            var deleteResponse = await sqsClient.DeleteMessageAsync(request, cancelToken).ConfigureAwait(false);

            // TODO: clarify about the status code
            if (deleteResponse.HttpStatusCode != HttpStatusCode.OK)
                _logger.LogWarning("Failed to delete message {MessageId} with HTTP status {HttpStatusCode}",
                    msg.MessageId, deleteResponse.HttpStatusCode);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete message {MessageId} withe exception {Exception}",
                msg.MessageId, e.ToString());
        }
    }
}