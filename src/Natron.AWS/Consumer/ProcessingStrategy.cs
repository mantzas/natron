namespace Natron.AWS.Consumer;

public enum ProcessingStrategy
{
    /// <summary>
    /// Crashes the service when batch processing fails by rethrowing the exception.
    /// This is the default behavior.
    /// </summary>
    Crash,

    /// <summary>
    /// Logs the error and continues processing subsequent batches when batch processing fails.
    /// WARNING: Failed messages will become visible again after the visibility timeout expires,
    /// potentially causing reprocessing loops for poison messages.
    /// </summary>
    LogAndContinue
}
