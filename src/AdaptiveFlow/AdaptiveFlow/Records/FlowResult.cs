namespace AdaptiveFlow;

/// <summary>
/// Represents the result of a flow execution, including success status, error details, and optional results.
/// </summary>
/// <param name="Success">Indicates whether the flow execution was successful.</param>
/// <param name="Error">The error details if the execution failed. Null if successful.</param>
/// <param name="Results">The optional results produced by the flow execution. Null if no results were generated.</param>
public record FlowResult(bool Success, FlowError? Error = null, IReadOnlyDictionary<string, object?>? Results = null)
{
    /// <summary>
    /// Creates a successful <see cref="FlowResult"/> with the specified results.
    /// </summary>
    /// <param name="results">A dictionary containing the results of the flow execution.</param>
    /// <returns>A successful <see cref="FlowResult"/> instance with the provided results.</returns>
    public static FlowResult Ok(IReadOnlyDictionary<string, object?> results) => new(true, null, results);

    /// <summary>
    /// Creates a successful <see cref="FlowResult"/> with an empty result set.
    /// </summary>
    /// <returns>A successful <see cref="FlowResult"/> instance with no results.</returns>
    public static FlowResult Ok() => new(true, null, new Dictionary<string, object?>());

    /// <summary>
    /// Creates a failed <see cref="FlowResult"/> with the specified error details.
    /// </summary>
    /// <param name="error">The error details associated with the failed execution.</param>
    /// <returns>A failed <see cref="FlowResult"/> instance with the provided error.</returns>
    public static FlowResult Fail(FlowError error) => new(false, error);
}
