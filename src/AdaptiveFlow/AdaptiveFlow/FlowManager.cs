namespace AdaptiveFlow;

/// <summary>
/// Manages the execution of flow steps based on orchestration rules.
/// Handles dependency resolution, execution order, concurrency, and error capturing.
/// </summary>
public class FlowManager : IDisposable
{
    #region Dispose

    /// <summary>
    /// Destructor for the FlowManager class.
    /// Ensures that the Dispose method is called for cleanup when the object is finalized.
    /// </summary>
    ~FlowManager() => Dispose();

    /// <summary>
    /// Disposes of the resources used by the FlowManager.
    /// Clears all steps and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        _orchestrator.Dispose();
        _semaphore.Release();
        GC.SuppressFinalize(this);
    }

    #endregion Dispose

    #region Variables

    private readonly FlowOrchestrator _orchestrator;
    private readonly SemaphoreSlim _semaphore;

    #endregion Variables

    #region Construtor

    /// <summary>
    /// Initializes a new instance of the FlowManager class.
    /// </summary>
    /// <param name="orchestrator">The FlowOrchestrator containing the flow steps to execute.</param>
    /// <param name="maxParallelism">The maximum number of parallel executions allowed.</param>
    public FlowManager(FlowOrchestrator orchestrator, int maxParallelism = 4)
    {
        _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        _semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);

        ValidateSteps();
    }

    #endregion Construtor

    #region Validate

    /// <summary>
    /// Validates the flow steps ensuring that there are no duplicate names
    /// and that all dependencies are correctly defined.
    /// </summary>
    /// <exception cref="FlowOrchestrationException">Thrown when a duplicate step or missing dependency is found.</exception>
    private void ValidateSteps()
    {
        var steps = _orchestrator.GetSteps();
        var stepNames = new HashSet<FlowKey>();

        foreach (var step in steps)
        {
            if (!stepNames.Add(step.Name))
                throw new FlowOrchestrationException($"Duplicate step name detected: {step.Name}");

            foreach (var dependency in step.Dependencies)
            {
                if (!steps.Any(s => s.Name == dependency))
                    throw new FlowOrchestrationException($"Dependency '{dependency}' for step '{step.Name}' not found.");
            }
        }
    }

    #endregion Validate

    #region Execute

    /// <summary>
    /// Executes the flow steps according to their dependencies and conditions.
    /// Handles concurrency and collects the execution results.
    /// </summary>
    /// <param name="context">The execution context shared across steps.</param>
    /// <param name="cancellationToken">Token for cancelling the execution.</param>
    /// <returns>A list of FlowResult containing the execution outcome for each step.</returns>
    /// <exception cref="FlowOrchestrationException">Thrown when the flow cannot progress due to unresolved dependencies (deadlock).</exception>
    public async Task<List<FlowResult>> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        List<FlowStepDescriptor>? steps = [.. _orchestrator.GetSteps()];
        HashSet<FlowKey>? executedSteps = [];
        List<FlowResult>? results = [];

        while (executedSteps.Count < steps.Count)
        {
            List<FlowStepDescriptor> readySteps = [.. steps.Where(s => !executedSteps.Contains(s.Name)).Where(s => s.Dependencies.All(dep => executedSteps.Contains(dep))).Where(s => s.Condition == null || s.Condition(context))];

            if (readySteps.Count == 0)
                throw new FlowOrchestrationException("Cannot resolve dependencies. Deadlock or missing dependency detected.");

            List<Task<FlowResult>>? taskList = [];

            foreach (var stepDescriptor in readySteps)
            {
                async Task<FlowResult> ExecuteStepAsync(FlowStepDescriptor descriptor)
                {
                    await _semaphore.WaitAsync(cancellationToken);

                    try
                    {
                        var output = await descriptor.Step.ExecuteAsync(context, cancellationToken);
                        return new FlowResult(descriptor.Name, true, output);
                    }
                    catch (Exception ex)
                    {
                        return new FlowResult(descriptor.Name, false, null, new FlowManagerException($"Step {descriptor.Name} throws an exception.", ex));
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                taskList.Add(ExecuteStepAsync(stepDescriptor));
            }

            var stepResults = await Task.WhenAll(taskList);
            results.AddRange(stepResults);
        }

        return results;
    }

    #endregion Execute
}