# Class `FlowManager`

Manages the execution of a flow based on a configured sequence of steps, supporting both synchronous and asynchronous processing. It uses a bounded channel for queued execution, a semaphore for concurrency control, and parallel execution for specified steps. Execution results are returned as a `FlowResult` to indicate success or failure, including typed results from steps. Optional logging and an injectable channel processor enhance flexibility and testability.

## Example

```csharp
var config = new FlowConfiguration().AddStep(new LogStep(), "LogStep");
var manager = new FlowManager(config);
await manager.StartProcessingAsync(); // Starts manual processing
var result = await manager.RunAsync(new FlowContext());
if (result.Success) Console.WriteLine("Flow completed: " + result.Result);
```

## Fields

- **`_config`**: (`FlowConfiguration`) The flow configuration defining the steps.
- **`_channel`**: (`Channel<FlowContext>`) A bounded channel for queuing flow contexts.
- **`_semaphore`**: (`SemaphoreSlim`) Controls the maximum number of concurrent flows.
- **`_maxParallelism`**: (`int`) The maximum number of parallel steps within a flow.
- **`_logger`**: (`ILogger<FlowManager>?`) Optional logger for execution events.
- **`_channelProcessor`**: (`IChannelProcessor`) Processor for handling queued contexts.

## Constructor

```csharp
public FlowManager(FlowConfiguration config, ILogger<FlowManager>? logger = null, IChannelProcessor? channelProcessor = null, int maxConcurrency = 5, int maxParallelism = 4, int channelCapacity = 1000)
```

Initializes a new `FlowManager` instance with the specified configuration, optional logger, and concurrency settings. Does not start processing automatically; use `StartProcessingAsync` to begin channel processing.

#### Parameters
- **`config`** (`FlowConfiguration`): The flow configuration defining the steps to execute.
- **`logger`** (`ILogger<FlowManager>?`): Optional logger instance for tracking events and errors. Defaults to `null`.
- **`channelProcessor`** (`IChannelProcessor?`): Processor for handling queued contexts. Defaults to `DefaultChannelProcessor`.
- **`maxConcurrency`** (`int`): Maximum number of flows that can run concurrently. Defaults to `5`.
- **`maxParallelism`** (`int`): Maximum number of parallel steps within a flow. Defaults to `4`.
- **`channelCapacity`** (`int`): Maximum number of contexts that can be queued. Defaults to `1000`.

---

## Methods

### StartProcessingAsync

```csharp
public Task StartProcessingAsync()
```

Starts processing queued contexts asynchronously using the configured `IChannelProcessor`. This method must be called explicitly to begin background processing, enhancing testability by avoiding automatic startup.

#### Returns
- `Task`: A task representing the asynchronous processing of the channel.

---

### EnqueueAsync

```csharp
public async Task EnqueueAsync(FlowContext context, CancellationToken cancellationToken = default)
```

Enqueues a flow context for asynchronous execution through the channel. If the channel is full, waits until space is available.

#### Parameters
- **`context`** (`FlowContext`): The context to enqueue for processing.
- **`cancellationToken`** (`CancellationToken`): Token to signal cancellation. Defaults to `default`.

#### Exceptions
- `ChannelClosedException`: Thrown if the channel is closed during enqueuing.

#### Example Behavior
- Logs enqueuing if a logger is present.
- Writes the context to the channel asynchronously.

---

### RunAsync

```csharp
public async Task<FlowResult> RunAsync(FlowContext context, CancellationToken cancellationToken = default)
```

Executes the flow synchronously with the provided context, blocking until completion.

#### Parameters
- **`context`** (`FlowContext`): The context to execute the flow with.
- **`cancellationToken`** (`CancellationToken`): Token to signal cancellation. Defaults to `default`.

#### Returns
- `Task<FlowResult>`: A task yielding the execution result.

---

### ExecuteFlowAsync (Protected Internal)

```csharp
protected internal async Task<FlowResult> ExecuteFlowAsync(FlowContext context, CancellationToken cancellationToken)
```

Executes the configured flow steps for a given context, respecting dependencies, concurrency, and parallelism settings. Returns a `FlowResult` with the outcome and logs execution details if a logger is provided.

#### Parameters
- **`context`** (`FlowContext`): The context for the flow execution.
- **`cancellationToken`** (`CancellationToken`): Token to signal cancellation.

#### Returns
- `Task<FlowResult>`: A task yielding the execution result.

#### Behavior
- Acquires a semaphore slot to limit concurrency.
- Executes steps based on conditions, dependencies, and parallelism settings.
- Collects results in a `ConcurrentBag<object>`.
- Handles deadlocks, cancellations, and exceptions with appropriate logging.

#### Exceptions
- `OperationCanceledException`: Returns a failed `FlowResult` if canceled.
- `Exception`: Returns a failed `FlowResult` with the error message.

---

### GetChannelReader (Protected Internal)

```csharp
protected internal ChannelReader<FlowContext> GetChannelReader()
```

Provides access to the channel reader for processing queued contexts. Intended for use by `IChannelProcessor` implementations.

#### Returns
- `ChannelReader<FlowContext>`: The reader for the channel.

---

### GetLogger (Protected Internal)

```csharp
protected internal ILogger<FlowManager>? GetLogger()
```

Provides access to the logger instance for logging execution events. Intended for use by `IChannelProcessor` implementations and internal logic.

#### Returns
- `ILogger<FlowManager>?`: The logger instance, or `null` if not provided.