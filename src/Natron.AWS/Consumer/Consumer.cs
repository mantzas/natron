using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SQS.Util;
using Microsoft.Extensions.Logging;
using ValidDotNet;

namespace Natron.AWS.Consumer;

public class Consumer : IComponent
{
    private readonly IAmazonSQS _client;

    private readonly Config _config;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public Consumer(ILoggerFactory loggerFactory, IAmazonSQS client, Config config)
    {
        _client = client.ThrowIfNull();
        _config = config.ThrowIfNull();
        _loggerFactory = loggerFactory.ThrowIfNull();
        _logger = _loggerFactory.CreateLogger<Consumer>();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = await GetMessagesAsync(_client, cancellationToken);

            if (messages == null)
            {
                _logger.LogDebug("Received null messages from SQS");
                await Task.Delay(TimeSpan.FromSeconds(_config.WaitTimeSeconds), cancellationToken);
                continue;
            }
            if (messages.Count == 0)
            {
                _logger.LogDebug("No messages received from SQS");
                await Task.Delay(TimeSpan.FromSeconds(_config.WaitTimeSeconds), cancellationToken);
                continue;
            }
            _logger.LogDebug("Received {MessageCount} messages from SQS", messages.Count);

            var batch = Batch.From(_loggerFactory, cancellationToken, _client, _config.QueueUrl, messages);

            try
            {
                await _config.ProcessFunc(batch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SQS batch");

                if (_config.ProcessingStrategy == ProcessingStrategy.Crash)
                {
                    _logger.LogCritical("ProcessingStrategy is set to Crash. Rethrowing exception");
                    throw;
                }

                _logger.LogWarning("ProcessingStrategy is set to LogAndContinue. Continuing to next batch");
            }
        }
    }

    private async Task<List<Amazon.SQS.Model.Message>> GetMessagesAsync(IAmazonSQS client,
        CancellationToken cancellationToken)
    {
        var request = new ReceiveMessageRequest
        {
            MessageAttributeNames = [SQSConstants.ATTRIBUTE_ALL],
            MessageSystemAttributeNames = [SQSConstants.ATTRIBUTE_CREATED_TIMESTAMP],
            QueueUrl = _config.QueueUrl,
            MaxNumberOfMessages = _config.MaxNumberOfMessages,
            VisibilityTimeout = _config.VisibilityTimeout,
            WaitTimeSeconds = _config.WaitTimeSeconds
        };

        var response = await client.ReceiveMessageAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.HttpStatusCode == HttpStatusCode.OK) return response.Messages;

        _logger.LogError("Failed to receive messages with HTTP status code {ResponseHttpStatusCode}",
            response.HttpStatusCode);
        return [];
    }
}
