# Record `FlowResult`

Represents the result of a flow execution, indicating success or failure along with optional data or error details.

## Example

```csharp
var result = new FlowResult(true, Result: "Operation completed");
if (result.Success) Console.WriteLine(result.Result); // Outputs: Operation completed
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

- **Type**: `object?`
- **Description**: Optional data produced by the flow. Null if no data is returned or on failure.
- **Default**: `null`