using ValidDotNet;

namespace Natron.Consumer.AWS.SQS;

public sealed class Config
{
    public Config(string queueUrl, Func<Batch, Task> processFunc, int visibilityTimeout = 10,
        int waitTimeSeconds = 10,
        int maxNumberOfMessages = 20, int statsInterval = 10)
    {
        QueueUrl = queueUrl.ThrowIfNullOrWhitespace(nameof(queueUrl));
        ProcessFunc = processFunc.ThrowIfNull(nameof(processFunc));
        MaxNumberOfMessages = maxNumberOfMessages.ThrowIfLessOrEqual(nameof(maxNumberOfMessages), 0);
        WaitTimeSeconds = waitTimeSeconds.ThrowIfLessOrEqual(nameof(waitTimeSeconds), 0);
        VisibilityTimeout = visibilityTimeout.ThrowIfLessOrEqual(nameof(visibilityTimeout), 0);
        StatsInterval = statsInterval.ThrowIfLessOrEqual(nameof(statsInterval), 0);
    }

    public int VisibilityTimeout { get; }
    public int WaitTimeSeconds { get; }
    public int MaxNumberOfMessages { get; }
    public string QueueUrl { get; }
    public Func<Batch, Task> ProcessFunc { get; }
    public int StatsInterval { get; }
}