namespace AdaptiveFlow;

/// <summary>
/// Represents an error that occurred during the execution of a flow.
/// Includes details such as an error code, message, step name (if applicable), and an optional inner exception.
/// </summary>
/// <param name="Code">The error code representing the type of error.</param>
/// <param name="Message">A descriptive message explaining the error.</param>
/// <param name="StepName">The name of the step where the error occurred, if applicable. Null if not step-specific.</param>
/// <param name="InnerException">The optional inner exception providing additional context for the error.</param>
public record FlowError(string Code, string Message, string? StepName = null, Exception? InnerException = null)
{
    /// <summary>
    /// Creates a <see cref="FlowError"/> indicating a deadlock occurred due to unexecuted steps.
    /// </summary>
    /// <param name="unexecutedSteps">A comma-separated list of steps that were not executed due to the deadlock.</param>
    /// <returns>A <see cref="FlowError"/> instance representing the deadlock.</returns>
    public static FlowError Deadlock(string unexecutedSteps) => new("DEADLOCK", $"Deadlock detected. Remaining steps: {unexecutedSteps}");

    /// <summary>
    /// Creates a <see cref="FlowError"/> indicating an exception occurred during the execution of a specific step.
    /// </summary>
    /// <param name="stepName">The name of the step where the exception occurred.</param>
    /// <param name="ex">The exception that occurred.</param>
    /// <returns>A <see cref="FlowError"/> instance representing the exception.</returns>
    public static FlowError Exception(string stepName, Exception ex) => new("EXCEPTION", $"Exception occurred in step '{stepName}': {ex.Message}", stepName, ex);

    /// <summary>
    /// Creates a <see cref="FlowError"/> indicating that the flow execution was cancelled.
    /// </summary>
    /// <returns>A <see cref="FlowError"/> instance representing the cancellation.</returns>
    public static FlowError Cancelled() => new("CANCELLED", "Flow execution was cancelled.");

    /// <summary>
    /// Creates a <see cref="FlowError"/> indicating a configuration issue in the flow setup.
    /// </summary>
    /// <param name="message">A descriptive message explaining the configuration error.</param>
    /// <returns>A <see cref="FlowError"/> instance representing the configuration error.</returns>
    public static FlowError Configuration(string message) => new("CONFIG_ERROR", message);       
}