# Interface `IFlowStepWrapper`

Defines a generic wrapper interface for flow steps, enabling the execution of both void steps (`IFlowStep`) and typed response steps (`IFlowStep<TResponse>`) within the `FlowManager` pipeline. This interface standardizes the execution of flow steps by providing a common method that returns an `object` representing the step's result (or `null` for void steps), allowing the `FlowConfiguration` and `FlowManager` to handle diverse step types uniformly. Implementations such as `VoidFlowStepWrapper` and `TypedFlowStepWrapper<TResponse>` use this interface to integrate their specific step logic into the flow.

## Example

```csharp
public class LogStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine("Logging...");
        await Task.CompletedTask;
    }
}

public class ComputeStep : IFlowStep<int>
{
    public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        return 42;
    }
}

IFlowStepWrapper voidWrapper = new VoidFlowStepWrapper(new LogStep());
IFlowStepWrapper typedWrapper = new TypedFlowStepWrapper<int>(new ComputeStep());
var context = new FlowContext();
var voidResult = await voidWrapper.ExecuteAsync(context, CancellationToken.None); // Returns null
var typedResult = await typedWrapper.ExecuteAsync(context, CancellationToken.None); // Returns 42
Console.WriteLine($"Void: {voidResult}, Typed: {typedResult}");
```

## Methods

### ExecuteAsync

```csharp
Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
```

Executes the wrapped flow step asynchronously using the provided context and cancellation token. Returns the step's result as an `object` (for typed steps) or `null` (for void steps). This method enables the `FlowManager` to process all steps consistently, collecting results into a `FlowResult` for further processing or reporting.

#### Parameters
- **`context`** (`FlowContext`): The execution context containing shared data for the flow step.
- **`cancellationToken`** (`CancellationToken`): A token to signal cancellation of the operation.

#### Returns
- `Task<object?>`: A task yielding the result of the step as an `object`, or `null` if the step does not produce a result.

#### Exceptions
- `Exception`: Thrown if the wrapped step encounters an error during execution.

#### Example

```csharp
var context = new FlowContext();
IFlowStepWrapper wrapper = new TypedFlowStepWrapper<string>(new GreetingStep());
var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
Console.WriteLine(result); // Outputs: "Hello, World!" (or null for void steps)
```