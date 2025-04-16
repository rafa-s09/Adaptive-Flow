namespace AdaptiveFlow;

public record FlowError(string Code, string Message, string? StepName = null, Exception? InnerException = null)
{
    public static FlowError Deadlock(string unexecutedSteps) => new("DEADLOCK", $"Deadlock detected. Remaining steps: {unexecutedSteps}");

    public static FlowError Exception(string stepName, Exception ex) => new("EXCEPTION", $"Exception occurred in step '{stepName}': {ex.Message}", stepName, ex);

    public static FlowError Cancelled() => new("CANCELLED", "Flow execution was cancelled.");

    public static FlowError Configuration(string message) => new("CONFIG_ERROR", message);       
}