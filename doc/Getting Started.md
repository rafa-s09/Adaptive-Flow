# Getting Started

This guide will help you get started with ***AdaptiveFlow*** in your project in just a few minutes.

## Prerequisites

- **.NET:** Version 9.0 or higher.
- **Dependencies:** The package uses `Microsoft.Extensions.Logging` and `System.Threading.Channels`. Ensure these dependencies are available in your project. Add them if needed:
    ```bash 
    dotnet add package Microsoft.Extensions.Logging
    ```

## Installation

**1. Add the Package:**
- Install via NuGet:
    ```bash
    dotnet add package AdaptiveFlow
    ```
- Or manually reference the DLL in the .csproj file if using a local version:
    ```xml
    <ItemGroup>
        <Reference Include="AdaptiveFlow">
            <HintPath>path/to/AdaptiveFlow.dll</HintPath>
        </Reference>
    </ItemGroup>
    ```

**2. Basic Configuration:**
- Ensure you have an `IServiceProvider` set up. In ASP.NET Core applications, this is automatically provided via dependency injection.

## Execution Flow

1. **Initial Configuration:**
- The client creates an instance of `FlowConfiguration` and adds the desired steps, specifying conditions, dependencies, and parallelism options.
- Example:
    ```csharp
    var config = new FlowConfiguration()
    .AddStep(new LogStep(), "LogStep")
    .AddStep(new ComputeStep(), "ComputeStep", dependsOn: new[] {   "LogStep" }, isParallel: true);
    ```

2. **FlowManager Initialization:**
- The `FlowManager` is instantiated with the configuration, initializing a bounded pipeline for queuing contexts.
- Processing does not start automatically; the `StartProcessingAsync()` method must be called to start background processing.

3. **Enqueueing:**
- A `FlowContext` is created and populated with initial data if necessary, and enqueued onto the channel via `EnqueueAsync`.
- Example:
    ```csharp
    var context = new FlowContext();
    context.Data.TryAdd("UserId", 123);
    await manager.EnqueueAsync(context);
    ```

4. **Processing:**
- The `IChannelProcessor` (default: `DefaultChannelProcessor`) reads the channel contexts and invokes `ExecuteFlowAsync` on the `FlowManager`.
- The `SemaphoreSlim` controls concurrency, limiting the number of flows executed simultaneously.

5. **Step Execution:**
- The `FlowManager` analyzes the configured steps:
- Checks dependencies and conditions to determine which steps can be executed. - Steps marked as `IsParallel` are executed in parallel using `Parallel.ForEachAsync`, respecting the `maxParallelism` limit.
- Sequential steps are executed one at a time, in the order allowed by the dependencies.
- Results of typed steps (`IFlowStep<TResponse>`) are collected in a list; steps without a return (`IFlowStep`) return `null`.

6. **Conclusion:**
- If all steps are executed successfully, a `FlowResult` is returned with `Success = true`, containing the context data (`ContextData`) and the step results (`StepResults`).
- If a deadlock (unresolved dependencies), cancellation or exception occurs, the `FlowResult` indicates failure with an error message.
- Example:
    ```csharp
    var result = await manager.RunAsync(context);
    if (result.Success)
     Console.WriteLine("Success: " + result.Result.StepResults.Count + " results");
    else
     Console.WriteLine("Failed: " + result.ErrorMessage);
     ```
     
## Complete Example

```csharp
var config = new FlowConfiguration()
    .AddStep(new LogStep(), "LogStep")
    .AddStep(new ComputeStep(), "ComputeStep", dependsOn: new[] { "LogStep" });

var manager = new FlowManager(config);
await manager.StartProcessingAsync();

var context = new FlowContext();
context.Data.TryAdd("Input", "Teste");
await manager.EnqueueAsync(context);

public class LogStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine("Log: " + context.Data["Input"]);
        await Task.Delay(100, cancellationToken);
    }
}

public class ComputeStep : IFlowStep<int>
{
    public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(50, cancellationToken);
        return 42;
    }
}
```