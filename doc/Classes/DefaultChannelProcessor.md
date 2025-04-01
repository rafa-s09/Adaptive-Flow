# Class `DefaultChannelProcessor`

Provides a default implementation of `IChannelProcessor` for processing flow contexts in the `FlowManager`. Reads contexts from the channel and executes them sequentially, logging results if a logger is available.

## Implements
- `IChannelProcessor`

## Methods

### ProcessAsync

```csharp
public async Task ProcessAsync(FlowManager manager)
```

Processes flow contexts from the channel of the specified `FlowManager`. Continues reading and executing contexts until the channel is closed or an unhandled exception occurs.

#### Parameters
- **`manager`** (`FlowManager`): The `FlowManager` instance to process contexts for.

#### Returns
- `Task`: A task representing the asynchronous processing of the channel.

#### Behavior
- Retrieves the `ChannelReader<FlowContext>` from the `FlowManager`.
- Reads contexts sequentially using `WaitToReadAsync` and `TryRead`.
- Executes each context using `manager.ExecuteFlowAsync`.
- Logs warnings for failed executions or information for successful ones if a logger is available.

#### Example Implementation (Implicit)

```csharp
ChannelReader<FlowContext> channelReader = manager.GetChannelReader();
while (await channelReader.WaitToReadAsync())
{
    if (channelReader.TryRead(out var context))
    {
        FlowResult flowResult = await manager.ExecuteFlowAsync(context, CancellationToken.None);
        if (!flowResult.Success)                
            manager.GetLogger()?.LogWarning("Flow execution completed with failure: {Error}", flowResult.ErrorMessage);                
        else                
            manager.GetLogger()?.LogInformation("Flow execution completed successfully.");                
    }
}
```