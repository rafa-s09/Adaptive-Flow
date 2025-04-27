namespace AdaptiveFlow.Exceptions;

/// <summary>
/// Represents an exception specific to flow orchestration errors.
/// </summary>
public class FlowOrchestrationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the FlowOrchestrationException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public FlowOrchestrationException(string message) : base(message) { }
}