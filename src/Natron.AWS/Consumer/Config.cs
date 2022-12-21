using ValidDotNet;

namespace Natron.AWS.Consumer;

public sealed class Config
{
    public Config(string queueUrl, Func<Batch, Task> processFunc, int visibilityTimeout = 10,
        int waitTimeSeconds = 10,
        int maxNumberOfMessages = 20, int statsInterval = 10)
    {
        QueueUrl = queueUrl.ThrowIfNullOrWhitespace();
        ProcessFunc = processFunc.ThrowIfNull();
        MaxNumberOfMessages = maxNumberOfMessages.ThrowIfLessOrEqual(0);
        WaitTimeSeconds = waitTimeSeconds.ThrowIfLessOrEqual(0);
        VisibilityTimeout = visibilityTimeout.ThrowIfLessOrEqual(0);
        StatsInterval = statsInterval.ThrowIfLessOrEqual(0);
    }

    public int VisibilityTimeout { get; }
    public int WaitTimeSeconds { get; }
    public int MaxNumberOfMessages { get; }
    public string QueueUrl { get; }
    public Func<Batch, Task> ProcessFunc { get; }
    public int StatsInterval { get; }
}