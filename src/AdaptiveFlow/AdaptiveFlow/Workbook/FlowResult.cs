namespace AdaptiveFlow.Workbook;

/// <summary>
/// Represents the result of a flow step execution, including its success status,
/// optional output content, and any exception that may have occurred.
/// </summary>
/// <param name="StepName">The unique name of the executed step.</param>
/// <param name="Success">Indicates whether the step execution was successful.</param>
/// <param name="Content">Optional content or result produced by the step execution.</param>
/// <param name="Message">Optional exception describing an error that occurred during execution.</param>
public record FlowResult(FlowKey StepName, bool Success, object? Content = null, Exception? Message = null);
