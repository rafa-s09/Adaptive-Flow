namespace AdaptiveFlow;

/// <summary>
/// Manages the execution of a flow based on a configured sequence of steps, supporting both synchronous and asynchronous processing.
/// It uses a channel for queued execution, a semaphore for concurrency control, and parallel execution for specified steps.
/// Execution results are returned as a <see cref="FlowResult"/> to indicate success or failure.
/// <br/><br/>
/// Example:
/// <code>
/// var config = new FlowConfiguration().AddStep(new LogStep());
/// var manager = new FlowManager(config);
/// var result = await manager.RunAsync(new FlowContext());
/// if (result.Success) Console.WriteLine("Flow completed successfully");
/// else Console.WriteLine($"Flow failed: {result.ErrorMessage}");
/// </code>
/// </summary>
public class FlowManager
{
    private readonly FlowConfiguration _config;
    private readonly Channel<FlowContext> _channel = Channel.CreateUnbounded<FlowContext>();
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxParallelism;

    /// <summary>
    /// Initializes a new FlowManager instance with the specified configuration and concurrency settings.
    /// Starts a background task to process queued contexts.
    /// </summary>
    /// <param name="config">The flow configuration defining the steps to execute.</param>
    /// <param name="maxConcurrency">The maximum number of flows that can run concurrently. Defaults to 5.</param>
    /// <param name="maxParallelism">The maximum number of parallel steps within a flow. Defaults to 4.</param>
    public FlowManager(FlowConfiguration config, int maxConcurrency = 5, int maxParallelism = 4)
    {
        _config = config;
        _semaphore = new SemaphoreSlim(maxConcurrency); 
        _maxParallelism = maxParallelism; 
        Task.Run(() => ProcessChannelAsync()); 
    }

    /// <summary>
    /// Enqueues a flow context for asynchronous execution through the channel.
    /// This allows the caller to continue without waiting for the flow to complete.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// context.Data.TryAdd("Key", "Value");
    /// await manager.EnqueueAsync(context);
    /// </code>
    /// </summary>
    /// <param name="context">The flow context to enqueue for execution.</param>
    /// <param name="cancellationToken">A token to cancel the enqueue operation if needed.</param>
    /// <returns>A task representing the asynchronous enqueue operation.</returns>
    public async Task EnqueueAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _channel.Writer.WriteAsync(context, cancellationToken);
    }

    /// <summary>
    /// Continuously processes flow contexts from the channel in the background.
    /// Reads queued contexts and triggers their execution.
    /// </summary>
    private async Task ProcessChannelAsync()
    {
        while (await _channel.Reader.WaitToReadAsync())
        {
            if (_channel.Reader.TryRead(out var context))            
                await ExecuteFlowAsync(context, CancellationToken.None);
        }
    }

    /// <summary>
    /// Executes the configured flow steps for a given context, respecting concurrency and parallelism settings.
    /// Parallel steps are executed concurrently up to the maxParallelism limit, while sequential steps run in order.
    /// </summary>
    /// <param name="context">The flow context containing data for the execution.</param>
    /// <param name="cancellationToken">A token to cancel the flow execution if needed.</param>
    /// <returns>A task representing the asynchronous execution of the flow.</returns>
    private async Task ExecuteFlowAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var parallelSteps = _config.Steps.Where(s => s.IsParallel && s.Condition(context)).Select(s => s.Step).ToList();
            var sequentialSteps = _config.Steps.Where(s => !s.IsParallel && s.Condition(context)).Select(s => (s.Step, s.Condition)).ToList();

            if (parallelSteps.Count != 0)
            {
                await Parallel.ForEachAsync(parallelSteps, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelism, CancellationToken = cancellationToken }, async (step, ct) => {
                    await step.ExecuteAsync(context, ct);
                });
            }

            foreach (var (step, _) in sequentialSteps)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await step.ExecuteAsync(context, cancellationToken);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Executes the flow synchronously with the provided context, blocking until completion.
    /// Useful for scenarios where immediate execution is required rather than queuing.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// await manager.RunAsync(context);
    /// Console.WriteLine(context.Data["Result"]);
    /// </code>
    /// </summary>
    /// <param name="context">The flow context to execute.</param>
    /// <param name="cancellationToken">A token to cancel the execution if needed.</param>
    /// <returns>A task representing the synchronous execution of the flow.</returns>
    public async Task RunAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        await ExecuteFlowAsync(context, cancellationToken);
    }
}