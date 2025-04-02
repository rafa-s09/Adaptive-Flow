namespace AdaptiveFlow;

/// <summary>
/// Encapsulates the inner results of a flow execution, providing access to shared context data and individual step outputs.
/// <para>
/// <see cref="ContextData"/> contains the shared state manipulated by steps during execution.
/// <see cref="StepResults"/> contains the direct outputs returned by typed steps (implementing <see cref="IFlowStep{TResponse}"/>).
/// </para>
/// <example>
/// Example usage:
/// <code>
/// var innerResults = flowResult.Result;
/// if (innerResults != null)
/// {
///     // Access shared context data
///     if (innerResults.ContextData.TryGetValue("Forecasts", out var forecastsObj) &amp;&amp; forecastsObj is IEnumerable&lt;WeatherForecast&gt; forecasts)
///     {
///         Console.WriteLine("Forecasts from ContextData: " + forecasts.Count());
///     }
///     // Access step results
///     var stepForecasts = innerResults.StepResults.OfType&lt;IEnumerable&lt;WeatherForecast&gt;&gt;().FirstOrDefault();
///     Console.WriteLine("Forecasts from StepResults: " + stepForecasts?.Count());
/// }
/// </code>
/// </example>
/// </summary>
/// <param name="ContextData">A dictionary containing shared data manipulated by steps during the flow execution.</param>
/// <param name="StepResults">A list of outputs returned by typed steps executed in the flow.</param>
public record FlowInnerResults(ConcurrentDictionary<string, object> ContextData, IList<object> StepResults);