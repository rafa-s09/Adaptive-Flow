# Class `VoidFlowStepWrapper`

Wraps an `IFlowStep` implementation to conform to the `IFlowStepWrapper` interface. This class adapts void flow steps (those that do not return a value) for use within the `FlowManager` and `FlowConfiguration`, enabling seamless execution alongside typed steps in a unified flow pipeline. Since `IFlowStep` does not produce a result, this wrapper returns `null` to maintain compatibility with the generic result collection mechanism of the `FlowResult`.

## Example

```csharp
public class LogStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine("Logging...");
        await Task.Delay(100, cancellationToken);
    }
}

var wrapper = new VoidFlowStepWrapper(new LogStep());
var context = new FlowContext();
var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
Console.WriteLine(result == null); // Outputs: True
```

## Constructor

```csharp
public VoidFlowStepWrapper(IFlowStep step)
```

Initializes a new instance of the `VoidFlowStepWrapper` class with the specified void flow step.

#### Parameters
- **`step`** (`IFlowStep`): The `IFlowStep` instance to wrap. Must not be null.

#### Exceptions
- `ArgumentNullException`: Thrown if `step` is null.

---

## Methods

### ExecuteAsync

```csharp
public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
```

Executes the wrapped `IFlowStep` asynchronously and returns `null` as the result. This method ensures that void steps can be executed within the `FlowManager` pipeline, maintaining compatibility with the `IFlowStepWrapper` interface used to collect results. Since the wrapped step does not produce a value, `null` is returned to indicate no result.

#### Parameters
- **`context`** (`FlowContext`): The execution context containing shared data for the flow step.
- **`cancellationToken`** (`CancellationToken`): A token to signal cancellation of the operation.

#### Returns
- `Task<object?>`: A task yielding `null`, as the wrapped step does not produce a result.

#### Exceptions
- `Exception`: Thrown if the wrapped step encounters an error during execution.

#### Example

```csharp
var context = new FlowContext();
context.Data.TryAdd("Message", "Starting process");
var wrapper = new VoidFlowStepWrapper(new LogStep());
var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
Console.WriteLine(result == null ? "No result" : result); // Outputs: No result
```