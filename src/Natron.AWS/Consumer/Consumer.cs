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

    public async Task RunAsync(CancellationToken cancelToken)
    {
        await Task.Factory.StartNew(async () => await GetStatsAsync(_client, cancelToken),
            TaskCreationOptions.LongRunning);


        while (!cancelToken.IsCancellationRequested)
        {
            var messages = await GetMessagesAsync(_client, cancelToken);

            if (messages.Count == 0) continue;

            var batch = Batch.From(_loggerFactory, cancelToken, _client, _config.QueueUrl, messages);

            await _config.ProcessFunc(batch);
        }
    }

    private async Task<List<Amazon.SQS.Model.Message>> GetMessagesAsync(IAmazonSQS client,
        CancellationToken cancelToken)
    {
        var request = new ReceiveMessageRequest
        {
            MessageAttributeNames = new List<string> { SQSConstants.ATTRIBUTE_ALL },
            AttributeNames = new List<string> { SQSConstants.ATTRIBUTE_CREATED_TIMESTAMP },
            QueueUrl = _config.QueueUrl,
            MaxNumberOfMessages = _config.MaxNumberOfMessages,
            VisibilityTimeout = _config.VisibilityTimeout,
            WaitTimeSeconds = _config.WaitTimeSeconds
        };

        var response = await client.ReceiveMessageAsync(request, cancelToken).ConfigureAwait(false);

        // TODO: clarify about the status code
        if (response.HttpStatusCode == HttpStatusCode.OK) return response.Messages;

        _logger.LogError("Failed to receive messages with HTTP status code {ResponseHttpStatusCode}",
            response.HttpStatusCode);
        return new List<Amazon.SQS.Model.Message>();
    }

    private async Task GetStatsAsync(IAmazonSQS client, CancellationToken cancelToken)
    {
        try
        {
            while (!cancelToken.IsCancellationRequested)
            {
                var request = new GetQueueAttributesRequest
                {
                    QueueUrl = _config.QueueUrl,
                    AttributeNames = new List<string>
                    {
                        SQSConstants.ATTRIBUTE_APPROXIMATE_NUMBER_OF_MESSAGES,
                        SQSConstants.ATTRIBUTE_APPROXIMATE_NUMBER_OF_MESSAGES_DELAYED,
                        SQSConstants.ATTRIBUTE_APPROXIMATE_NUMBER_OF_MESSAGES_NOT_VISIBLE
                    }
                };
                var response = await client.GetQueueAttributesAsync(request, cancelToken).ConfigureAwait(false);

                // TODO: check this out
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.StatsInterval), cancelToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get queue attributes");
        }
    }
}