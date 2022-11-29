using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SQS.Util;
using Microsoft.Extensions.Logging;
using Prometheus;
using ValidDotNet;

namespace Natron.Consumer.AWS.SQS;

public class Consumer : IComponent
{
    private static readonly Gauge MessageAgeGauge = Metrics
        .CreateGauge("consumer_sqs_message_age_seconds", "Messages age in seconds.", "queue");


    private static readonly Gauge QueueSizeGauge = Metrics
        .CreateGauge("consumer_sqs_queue_size", "Queue size.", "queue", "state");

    private readonly IAmazonSQS _client;

    private readonly Config _config;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public Consumer(ILoggerFactory loggerFactory, IAmazonSQS client, Config config)
    {
        _client = client.ThrowIfNull(nameof(client));
        _config = config.ThrowIfNull(nameof(config));
        _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
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

            _config.ProcessFunc(batch);
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

        _logger.LogError("Failed to receive messages with HTTP status code {HttpStatusCode}",
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
                    QueueSizeGauge.WithLabels(_config.QueueUrl, "available")
                        .Set(response.ApproximateNumberOfMessages);
                    QueueSizeGauge.WithLabels(_config.QueueUrl, "delayed")
                        .Set(response.ApproximateNumberOfMessagesDelayed);
                    QueueSizeGauge.WithLabels(_config.QueueUrl, "invisible")
                        .Set(response.ApproximateNumberOfMessagesNotVisible);
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