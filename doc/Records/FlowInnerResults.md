# Record `FlowInnerResults`

Encapsulates the inner results of a flow execution, providing access to shared context data and individual step outputs.

`ContextData` contains the shared state manipulated by steps during execution. `StepResults` contains the direct outputs returned by typed steps (implementing `IFlowStep<TResponse>`).

## Example

```csharp
var innerResults = flowResult.Result;
if (innerResults != null)
{
    // Access shared context data
    if (innerResults.ContextData.TryGetValue("Forecasts", out var forecastsObj) && forecastsObj is IEnumerable<WeatherForecast> forecasts)
    {
        Console.WriteLine("Forecasts from ContextData: " + forecasts.Count());
    }
    // Access step results
    var stepForecasts = innerResults.StepResults.OfType<IEnumerable<WeatherForecast>>().FirstOrDefault();
    Console.WriteLine("Forecasts from StepResults: " + stepForecasts?.Count());
}
```

## Parameters

### ContextData

- **Type**: `ConcurrentDictionary<string, object>`
- **Description**: A dictionary containing shared data manipulated by steps during the flow execution.

### StepResults

- **Type**: `IList<object>`
- **Description**: A list of outputs returned by typed steps executed in the flow.