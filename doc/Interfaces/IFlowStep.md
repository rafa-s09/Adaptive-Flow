# Interface `IFlowStep`

Defines a step in a flow that can be executed asynchronously. Implementations should perform a specific task within the flow and respect cancellation requests. If an error occurs, implementations should throw an exception to be handled by the flow manager.

## Example

```csharp
public class LogStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!context.Data.ContainsKey("Email")) throw new ArgumentException("Email is required");
        Console.WriteLine("Logging...");
        await Task.Delay(100, cancellationToken);
    }
}
```

## Methods

### ExecuteAsync

```csharp
Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
```

Executes the flow step asynchronously using the provided context.

#### Parameters
- **`context`** (`FlowContext`): The execution context containing shared data.
- **`cancellationToken`** (`CancellationToken`): A token to signal cancellation of the operation. Defaults to `default`.

#### Returns
- `Task`: A task representing the asynchronous execution of the step.

#### Exceptions
- `Exception`: Thrown if the step encounters an error during execution.

<br/><br/>

---

# Interface `IFlowStep<TResponse>`

Defines a step in a flow that returns a typed response upon execution. Useful for steps that produce a result to be consumed by subsequent steps or the caller. Implementations should throw exceptions if errors occur during execution, which will be captured by the flow manager.

## Type Parameters
- **`TResponse`**: The type of response returned by the step.

## Example

```csharp
public class ComputeStep : IFlowStep<int>
{
    public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!context.Data.ContainsKey("Input")) throw new ArgumentException("Input is required");
        await Task.Delay(50, cancellationToken);
        return 42;
    }
}
```

## Methods

### ExecuteAsync

```csharp
Task<TResponse> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
```

Executes the flow step asynchronously and returns a result of type `TResponse`.

#### Parameters
- **`context`** (`FlowContext`): The execution context containing shared data.
- **`cancellationToken`** (`CancellationToken`): A token to signal cancellation of the operation. Defaults to `default`.

#### Returns
- `Task<TResponse>`: A task yielding a result of type `TResponse` upon successful execution.

#### Exceptions
- `Exception`: Thrown if the step encounters an error during execution.