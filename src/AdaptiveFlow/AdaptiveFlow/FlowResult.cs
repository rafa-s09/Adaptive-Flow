namespace AdaptiveFlow;

/// <summary>
/// Represents the result of a flow execution, indicating success or failure along with optional data or error details.
/// <br/><br/>
/// Example:
/// <code>
/// var result = new FlowResult(true, Result: "Operation completed");
/// if (result.Success) Console.WriteLine(result.Result); // Outputs: Operation completed
/// </code>
/// </summary>
/// <param name="Success">Indicates whether the flow executed successfully.</param>
/// <param name="ErrorMessage">The error message if the flow failed. Null if successful.</param>
/// <param name="Result">Optional data produced by the flow. Null if no data is returned or on failure.</param>
public record FlowResult(bool Success, string? ErrorMessage = null, object? Result = null);
