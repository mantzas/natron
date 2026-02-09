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
    /// WARNING: When using auto-commit mode, failed messages will be marked as consumed and lost.
    /// Consider using manual commit mode if message loss is unacceptable.
    /// </summary>
    LogAndContinue
}
