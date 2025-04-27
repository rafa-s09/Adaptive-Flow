namespace AdaptiveFlow;

/// <summary>
/// Responsible for organizing and managing the flow steps, their conditions,
/// dependencies, and execution properties.
/// </summary>
public class FlowOrchestrator : IFlowOrchestrator, IDisposable
{
    #region Dispose

    /// <summary>
    /// Destructor for the FlowOrchestrator class.
    /// Ensures that the Dispose method is called for cleanup when the object is finalized.
    /// </summary>
    ~FlowOrchestrator() => Dispose();

    /// <summary>
    /// Disposes of the resources used by the FlowOrchestrator.
    /// Clears all steps and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        _steps.Clear();
        GC.SuppressFinalize(this);
    }

    #endregion Dispose

    #region Variables

    private readonly ConcurrentList<FlowStepDescriptor> _steps = new();

    #endregion

    #region Functions

    /// <summary>
    /// Adds a new step to the flow orchestration.
    /// </summary>
    /// <param name="step">The step to add.</param>
    /// <param name="name">The unique name of the step.</param>
    /// <param name="dependencies">Optional dependencies that must complete before this step executes.</param>
    /// <param name="condition">Optional condition that must be true for the step to execute.</param>
    /// <param name="allowParallelExecution">Indicates if the step can be executed in parallel with others.</param>
    public async Task AddStep(IFlowStep step, string name, IEnumerable<string>? dependencies = null, Func<FlowContext, bool>? condition = null, bool allowParallelExecution = false)
    {
        _steps.Add(new FlowStepDescriptor(new FlowKey(name), step, dependencies?.Select(dep => new FlowKey(dep)).ToList() ?? [], condition, allowParallelExecution));
        await Task.CompletedTask;
    }

    /// <summary>
    /// Adds a step that is allowed to execute in parallel with others.
    /// </summary>
    /// <param name="step">The step to add.</param>
    /// <param name="name">The unique name of the step.</param>
    /// <param name="dependencies">Optional dependencies that must complete before this step executes.</param>
    /// <param name="condition">Optional condition that must be true for the step to execute.</param>
    public async Task AddParallelStep(IFlowStep step, string name, IEnumerable<string>? dependencies = null, Func<FlowContext, bool>? condition = null)
    {
        await AddStep(step, name, dependencies, condition, allowParallelExecution: true);
    }

    /// <summary>
    /// Adds a step that will execute only if a specific condition is met.
    /// </summary>
    /// <param name="step">The step to add.</param>
    /// <param name="name">The unique name of the step.</param>
    /// <param name="condition">The condition that must be true for the step to execute.</param>
    /// <param name="dependencies">Optional dependencies that must complete before this step executes.</param>

    /// <param name="allowParallelExecution">Indicates if the step can be executed in parallel with others.</param>
    public async Task AddConditionalStep(IFlowStep step, string name, Func<FlowContext, bool> condition, IEnumerable<string>? dependencies = null, bool allowParallelExecution = false)
    {
        await AddStep(step, name, dependencies, condition, allowParallelExecution);
    }

    /// <summary>
    /// Adds a step that has mandatory dependencies to other steps.
    /// </summary>
    /// <param name="step">The step to add.</param>
    /// <param name="name">The unique name of the step.</param>
    /// <param name="dependencies">The list of step names this step depends on.</param>
    /// <param name="condition">Optional condition that must be true for the step to execute.</param>
    /// <param name="allowParallelExecution">Indicates if the step can be executed in parallel with others.</param>
    public async Task AddStepWithDependencies(IFlowStep step, string name, IEnumerable<string> dependencies, Func<FlowContext, bool>? condition = null, bool allowParallelExecution = false)
    {
        await AddStep(step, name, dependencies, condition, allowParallelExecution);
    }

    /// <summary>
    /// Retrieves all steps currently configured in the orchestrator.
    /// </summary>
    /// <returns>An enumerable collection of configured flow step descriptors.</returns>
    public IEnumerable<FlowStepDescriptor> GetSteps() => _steps.ToEnumerable();

    #endregion
}