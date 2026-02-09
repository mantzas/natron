namespace Natron.Kafka.Consumer;

public enum ProcessingStrategy
{
    /// <summary>
    /// Crashes the service when message processing fails by rethrowing the exception.
    /// This is the default behavior.
    /// </summary>
    Crash,

    /// <summary>
    /// Logs the error and continues processing subsequent messages when message processing fails.
    /// </summary>
    LogAndContinue
}
