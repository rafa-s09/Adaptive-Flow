namespace AdaptiveFlow;

/// <summary>
/// Represents the result of a flow execution, indicating success or failure along with optional inner results or error details.
/// <para>
/// The <see cref="Success"/> property indicates whether the flow completed successfully.
/// If successful, <see cref="Result"/> contains the <see cref="FlowInnerResults"/> with context data and step outputs.
/// If unsuccessful, <see cref="ErrorMessage"/> provides details about the failure.
/// </para>
/// <example>
/// Example usage:
/// <code>
/// var result = await flowManager.RunAsync(new FlowContext(), CancellationToken.None);
/// if (result.Success)
/// {
///     var forecasts = result.Result.StepResults.OfType&lt;IEnumerable&lt;WeatherForecast&gt;&gt;().FirstOrDefault();
///     Console.WriteLine($"Got {forecasts?.Count()} forecasts");
/// }
/// else
/// {
///     Console.WriteLine($"Flow failed: {result.ErrorMessage}");
/// }
/// </code>
/// </example>
/// </summary>
/// <param name="Success">Indicates whether the flow executed successfully.</param>
/// <param name="ErrorMessage">The error message if the flow failed. Null if successful.</param>
/// <param name="Result">The inner results of the flow execution, including context data and step outputs. Null if the flow failed.</param>
public record FlowResult(bool Success, string? ErrorMessage = null, FlowInnerResults? Result = null);
