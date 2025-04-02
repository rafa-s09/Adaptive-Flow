# Record `FlowResult`

Represents the result of a flow execution, indicating success or failure along with optional inner results or error details.

The `Success` property indicates whether the flow completed successfully. If successful, `Result` contains the `FlowInnerResults` with context data and step outputs. If unsuccessful, `ErrorMessage` provides details about the failure.

## Example

```csharp
var result = await flowManager.RunAsync(new FlowContext(), CancellationToken.None);
if (result.Success)
{
    var forecasts = result.Result.StepResults.OfType<IEnumerable<WeatherForecast>>().FirstOrDefault();
    Console.WriteLine($"Got {forecasts?.Count()} forecasts");
}
else
{
    Console.WriteLine($"Flow failed: {result.ErrorMessage}");
}
```

## Parameters

### Success

- **Type**: `bool`
- **Description**: Indicates whether the flow executed successfully.

### ErrorMessage

- **Type**: `string?`
- **Description**: The error message if the flow failed. Null if successful.
- **Default**: `null`

### Result

- **Type**: `FlowInnerResults?`
- **Description**: The inner results of the flow execution, including context data and step outputs. Null if the flow failed.
- **Default**: `null`