using ValidDotNet;

namespace Natron.AWS.Consumer;

public sealed class Config
{
    public Config(string queueUrl, Func<Batch, Task> processFunc, int visibilityTimeout = 10,
        int waitTimeSeconds = 10,
        int maxNumberOfMessages = 10,
        ProcessingStrategy processingStrategy = ProcessingStrategy.Crash)
    {
        QueueUrl = queueUrl.ThrowIfNullOrWhitespace();
        ProcessFunc = processFunc.ThrowIfNull();
        MaxNumberOfMessages = maxNumberOfMessages.ThrowIfLessOrEqual(0);
        WaitTimeSeconds = waitTimeSeconds.ThrowIfLessOrEqual(0);
        VisibilityTimeout = visibilityTimeout.ThrowIfLessOrEqual(0);

        if (!Enum.IsDefined(typeof(ProcessingStrategy), processingStrategy))
        {
            throw new ArgumentException($"Invalid ProcessingStrategy: {processingStrategy}",
                nameof(processingStrategy));
        }

        ProcessingStrategy = processingStrategy;
    }

    public int VisibilityTimeout { get; }
    public int WaitTimeSeconds { get; }
    public int MaxNumberOfMessages { get; }
    public string QueueUrl { get; }
    public Func<Batch, Task> ProcessFunc { get; }
    public ProcessingStrategy ProcessingStrategy { get; }
}