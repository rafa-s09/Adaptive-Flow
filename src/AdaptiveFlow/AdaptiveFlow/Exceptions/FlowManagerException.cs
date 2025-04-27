namespace AdaptiveFlow.Exceptions;

/// <summary>
/// Represents errors that occur during the execution or management of flow steps.
/// </summary>
public class FlowManagerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlowManagerException"/> class with a specified error message and an optional inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="exception">The exception that caused the current exception, or <c>null</c> if no inner exception is specified.</param>
    public FlowManagerException(string message, Exception? exception) : base(message, exception) { }
}

