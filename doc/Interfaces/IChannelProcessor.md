# Interface `IChannelProcessor`

Defines a processor for handling flow contexts from a channel asynchronously. Implementations are responsible for reading and executing queued contexts in the `FlowManager`. This abstraction enhances testability by allowing the channel processing logic to be mocked or customized.

## Example

```csharp
public class CustomProcessor : IChannelProcessor
{
    public async Task ProcessAsync(FlowManager manager)
    {
        while (await manager._channel.Reader.WaitToReadAsync())
        {
            if (manager._channel.Reader.TryRead(out var context))
            {
                await manager.ExecuteFlowAsync(context, CancellationToken.None);
            }
        }
    }
}
```

## Methods

### ProcessAsync

```csharp
Task ProcessAsync(FlowManager manager);
```

Processes flow contexts from the channel managed by the `FlowManager`.

#### Parameters
- **`manager`** (`FlowManager`): The `FlowManager` instance providing access to the channel and execution logic.

#### Returns
- `Task`: A task representing the asynchronous processing of the channel.