namespace AdaptiveFlow;

public class FlowManager
{
    private readonly FlowConfiguration _config;
    private readonly Channel<FlowContext> _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxParallelism;
    private readonly ILogger<FlowManager>? _logger; 
    private readonly IChannelProcessor _channelProcessor;

    public FlowManager(FlowConfiguration config, ILogger<FlowManager>? logger = null, IChannelProcessor? channelProcessor = null, int maxConcurrency = 5, int maxParallelism = 4, int channelCapacity = 1000)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
        _semaphore = new SemaphoreSlim(maxConcurrency);
        _maxParallelism = maxParallelism;
        _logger = logger;
        _channel = Channel.CreateBounded<FlowContext>(new BoundedChannelOptions(channelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        _channelProcessor = channelProcessor ?? new DefaultChannelProcessor();
    }

    public Task StartProcessingAsync()
    {
        return Task.Run(() => _channelProcessor.ProcessAsync(this));
    }

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

    public async Task<FlowResult> RunAsync(FlowContext context, CancellationToken cancellationToken = default) => await ExecuteFlowAsync(context, cancellationToken);

    protected internal async Task<FlowResult> ExecuteFlowAsync(FlowContext context, CancellationToken cancellationToken) 
    {
        if (_config.Steps.Count < 1)
            throw new InvalidOperationException("Cannot execute a flow with an empty step configuration. Add at least one step to the FlowConfiguration.");

        try
        {
            if (_logger?.IsEnabled(LogLevel.Information) == true)
                _logger.LogInformation("Starting flow execution with {StepCount} steps.", _config.Steps.Count);

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                List<object> results = [];
                object resultsLock = new();
                HashSet<string> executedSteps = new(StringComparer.Ordinal);
                List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> remainingSteps = [.. _config.Steps];
                List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> executableParallel = [.. remainingSteps.Where(s => s.IsParallel && s.Condition(context) && s.DependsOn.All(dep => executedSteps.Contains(dep)))];
                List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> executableSequential = [.. remainingSteps.Where(s => !s.IsParallel && s.Condition(context) && s.DependsOn.All(dep => executedSteps.Contains(dep)))];

                while (remainingSteps.Count != 0)
                {
                    executableParallel.Clear();
                    executableSequential.Clear();

                    foreach (var step in remainingSteps)
                    {
                        if (!step.Condition(context)) continue;
                        if (step.DependsOn.Any(dep => !executedSteps.Contains(dep))) continue;

                        if (step.IsParallel)
                            executableParallel.Add(step);
                        else
                            executableSequential.Add(step);
                    }

                    if (executableParallel.Count < 1 && executableSequential.Count < 1)
                    {
                        var unexecuted = string.Join(", ", remainingSteps.Select(s => s.StepName));
                        _logger?.LogError("Deadlock detected: remaining steps ({Unexecuted}) cannot be executed due to unmet dependencies.", unexecuted);
                        return FlowResult.Fail(FlowError.Deadlock(unexecuted));
                    }

                    if (executableParallel.Count > 0)
                    {
                        if (_logger?.IsEnabled(LogLevel.Debug) == true)
                            _logger.LogDebug("Executing {Count} parallel steps.", executableParallel.Count);

                        var tasks = executableParallel.Select(async s =>
                        {
                            var result = await s.Step.ExecuteAsync(context, cancellationToken);
                            if (result != null)                            
                                lock (resultsLock)                                
                                    results.Add(result);                        

                            lock (executedSteps)                            
                                executedSteps.Add(s.StepName);  
                        });
                        await Task.WhenAll(tasks);

                        remainingSteps.RemoveAll(s => executableParallel.Contains(s));
                    }

                    foreach (var s in executableSequential)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _logger?.LogDebug("Executing sequential step: {StepName}", s.StepName);
                        var result = await s.Step.ExecuteAsync(context, cancellationToken);
                        if (result != null)                        
                            lock (resultsLock)                            
                                results.Add(result);                            
                        
                        executedSteps.Add(s.StepName);
                        remainingSteps.Remove(s);
                        break;
                    }
                }

                if (_logger?.IsEnabled(LogLevel.Information) == true)
                    _logger.LogInformation("Flow execution completed successfully with {ResultCount} step results.", results.Count);
                
                return FlowResult.Ok(context.AsReadOnly("default"));
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger?.LogWarning(ex, "Flow execution was canceled.");
            return FlowResult.Fail(FlowError.Cancelled());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Flow execution failed.");
            return FlowResult.Fail(new FlowError("UNHANDLED_EXCEPTION", ex.Message, null, ex));
        }
    }

    public void EndChannel() => _channel.Writer.Complete();

    public Channel<FlowContext> GetChannel() => _channel;

    public ILogger<FlowManager>? GetLogger() => _logger;
}