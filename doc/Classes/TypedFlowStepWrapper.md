# Class `TypedFlowStepWrapper<TResponse>`

Wraps an `IFlowStep<TResponse>` implementation to conform to the `IFlowStepWrapper` interface. This class enables the integration of typed flow steps (those returning a specific `TResponse`) into the `FlowManager` and `FlowConfiguration`, allowing the execution of steps with return values alongside void steps in a unified flow pipeline. The wrapper converts the typed result into a generic `object` to be collected and returned as part of the flow's `FlowResult`.

## Type Parameters
- **`TResponse`**: The type of response returned by the wrapped `IFlowStep<TResponse>` implementation.

## Example

```csharp
public class ComputeStep : IFlowStep<int>
{
    public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        return 42;
    }
}

var wrapper = new TypedFlowStepWrapper<int>(new ComputeStep());
var context = new FlowContext();
var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
Console.WriteLine(result); // Outputs: 42
```

## Constructor

```csharp
public TypedFlowStepWrapper(IFlowStep<TResponse> step)
```

Initializes a new instance of the `TypedFlowStepWrapper<TResponse>` class with the specified typed flow step.

#### Parameters
- **`step`** (`IFlowStep<TResponse>`): The `IFlowStep<TResponse>` instance to wrap. Must not be null.

#### Exceptions
- `ArgumentNullException`: Thrown if `step` is null.

---

## Methods

### ExecuteAsync

```csharp
public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
```

Executes the wrapped `IFlowStep<TResponse>` asynchronously and returns its result as an `object`. This method bridges the typed step's execution to the generic `IFlowStepWrapper` interface, allowing the `FlowManager` to collect and include the result in the flow's `FlowResult`.

#### Parameters
- **`context`** (`FlowContext`): The execution context containing shared data for the flow step.
- **`cancellationToken`** (`CancellationToken`): A token to signal cancellation of the operation.

#### Returns
- `Task<object?>`: A task yielding the result of the wrapped step as an `object`, or null if no result is applicable.

#### Exceptions
- `Exception`: Thrown if the wrapped step encounters an error during execution.

#### Example

```csharp
var context = new FlowContext();
var wrapper = new TypedFlowStepWrapper<string>(new GreetingStep());
var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
Console.WriteLine(result); // Outputs: "Hello, World!"
```