namespace AdaptiveFlow;

/// <summary>
/// Manages the execution of a flow based on a configured sequence of steps, supporting both synchronous and asynchronous processing.
/// It uses a bounded channel for queued execution, a semaphore for concurrency control, and parallel execution for specified steps.
/// Execution results are returned as a <see cref="FlowResult"/> to indicate success or failure, including typed results from steps.
/// Optional logging and an injectable channel processor enhance flexibility and testability.
/// <br/><br/>
/// Example:
/// <code>
/// var config = new FlowConfiguration().AddStep(new LogStep(), "LogStep");
/// var manager = new FlowManager(config);
/// await manager.StartProcessingAsync(); // Inicia o processamento manualmente
/// var result = await manager.RunAsync(new FlowContext());
/// if (result.Success) Console.WriteLine("Flow completed: " + result.Result);
/// </code>
/// </summary>
public class FlowManager
{
    private readonly FlowConfiguration _config;
    private readonly Channel<FlowContext> _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxParallelism;
    private readonly ILogger<FlowManager>? _logger; 
    private readonly IChannelProcessor _channelProcessor;

    /// <summary>
    /// Initializes a new FlowManager instance with the specified configuration, optional logger, and concurrency settings.
    /// Does not start processing automatically; use <see cref="StartProcessingAsync"/> to begin channel processing.
    /// </summary>
    /// <param name="config">The flow configuration defining the steps to execute.</param>
    /// <param name="logger">An optional logger instance for tracking execution events and errors. Defaults to null (no logging).</param>
    /// <param name="channelProcessor">The processor for handling queued contexts. Defaults to <see cref="DefaultChannelProcessor"/>.</param>
    /// <param name="maxConcurrency">The maximum number of flows that can run concurrently. Defaults to 5.</param>
    /// <param name="maxParallelism">The maximum number of parallel steps within a flow. Defaults to 4.</param>
    /// <param name="channelCapacity">The maximum number of contexts that can be queued in the channel. Defaults to 1000.</param>
    public FlowManager(FlowConfiguration config, ILogger<FlowManager>? logger = null, IChannelProcessor? channelProcessor = null, int maxConcurrency = 5, int maxParallelism = 4, int channelCapacity = 1000)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
        _semaphore = new SemaphoreSlim(maxConcurrency);
        _maxParallelism = maxParallelism;
        _logger = logger;
        _channel = Channel.CreateBounded<FlowContext>(new BoundedChannelOptions(channelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        _channelProcessor = channelProcessor ?? new DefaultChannelProcessor();
    }

    /// <summary>
    /// Starts processing queued contexts asynchronously using the configured <see cref="IChannelProcessor"/>.
    /// This method must be called explicitly to begin background processing, enabling testability by avoiding automatic startup.
    /// </summary>
    /// <returns>A task representing the asynchronous processing of the channel.</returns>
    public Task StartProcessingAsync()
    {
        return Task.Run(() => _channelProcessor.ProcessAsync(this));
    }

    /// <summary>
    /// Enqueues a flow context for asynchronous execution through the channel.
    /// If the channel is full, waits until space is available.
    /// </summary>
    public async Task EnqueueAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger?.LogInformation("Enqueuing flow context for processing.");
        try
        {
            await _channel.Writer.WriteAsync(context, cancellationToken);
        }
        catch (ChannelClosedException ex)
        {
            _logger?.LogError(ex, "Failed to enqueue context: Channel is closed.");
            throw;
        }
    }

    /// <summary>
    /// Executes the flow synchronously with the provided context, blocking until completion.
    /// </summary>
    public async Task<FlowResult> RunAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        return await ExecuteFlowAsync(context, cancellationToken);
    }

    /// <summary>
    /// Executes the configured flow steps for a given context, respecting dependencies, concurrency, and parallelism settings.
    /// Returns a <see cref="FlowResult"/> with the outcome and logs execution details if a logger is provided.
    /// </summary>
    protected internal async Task<FlowResult> ExecuteFlowAsync(FlowContext context, CancellationToken cancellationToken) 
    {
        try
        {
            _logger?.LogInformation("Starting flow execution with {StepCount} steps.", _config.Steps.Count);
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                ConcurrentBag<object> results = [];
                HashSet<string> executedSteps = [];
                List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> remainingSteps = [.. _config.Steps];

                while (remainingSteps.Count != 0)
                {
                    List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> executableParallel = [.. remainingSteps.Where(s => s.IsParallel && s.Condition(context) && s.DependsOn.All(dep => executedSteps.Contains(dep)))];
                    List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> executableSequential = [.. remainingSteps.Where(s => !s.IsParallel && s.Condition(context) && s.DependsOn.All(dep => executedSteps.Contains(dep)))];

                    if (executableParallel.Count < 1 && executableSequential.Count < 1)
                    {
                        var unexecuted = string.Join(", ", remainingSteps.Select(s => s.StepName));
                        _logger?.LogError("Deadlock detected: remaining steps ({Unexecuted}) cannot be executed due to unmet dependencies.", unexecuted);
                        return new FlowResult(false, $"Deadlock detected: unexecuted steps - {unexecuted}");
                    }

                    if (executableParallel.Count != 0)
                    {
                        _logger?.LogDebug("Executing {Count} parallel steps.", executableParallel.Count);
                        await Parallel.ForEachAsync(executableParallel, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelism, CancellationToken = cancellationToken }, async (s, ct) =>
                        {
                            _logger?.LogDebug("Executing parallel step: {StepName}", s.StepName);
                            var result = await s.Step.ExecuteAsync(context, ct);
                            if (result != null) results.Add(result);
                            executedSteps.Add(s.StepName);
                        });
                        remainingSteps.RemoveAll(s => executableParallel.Contains(s));
                    }

                    foreach (var s in executableSequential)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _logger?.LogDebug("Executing sequential step: {StepName}", s.StepName);
                        var result = await s.Step.ExecuteAsync(context, cancellationToken);
                        if (result != null) results.Add(result);
                        executedSteps.Add(s.StepName);
                        remainingSteps.Remove(s);
                        break;
                    }
                }

                _logger?.LogInformation("Flow execution completed successfully with {ResultCount} step results.", results.Count);
                return new FlowResult(true, string.Empty, new FlowInnerResults(context.Data, results.ToList()));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger?.LogWarning(ex, "Flow execution was canceled.");
            return new FlowResult(false, "Flow was canceled");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Flow execution failed.");
            return new FlowResult(false, $"Flow execution failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Provides access to the channel reader for processing queued contexts.
    /// This method is intended for use by <see cref="IChannelProcessor"/> implementations.
    /// </summary>
    protected internal ChannelReader<FlowContext> GetChannelReader() => _channel.Reader;

    /// <summary>
    /// Provides access to the logger instance for logging execution events.
    /// This method is intended for use by <see cref="IChannelProcessor"/> implementations and internal logic.
    /// </summary>
    protected internal ILogger<FlowManager>? GetLogger() => _logger;
}