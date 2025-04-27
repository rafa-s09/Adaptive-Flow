namespace AdaptiveFlow.Interfaces;

/// <summary>
/// Defines the contract for a flow orchestrator responsible for managing
/// the organization, dependencies, and conditions of flow steps within a process.
/// </summary>
public interface IFlowOrchestrator
{

    /// <summary>
    /// Adds a new step to the flow orchestration.
    /// </summary>
    /// <param name="step">The step to add.</param>
    /// <param name="name">The unique name of the step.</param>
    /// <param name="dependencies">Optional dependencies that must complete before this step executes.</param>
    /// <param name="condition">Optional condition that must be true for the step to execute.</param>
    /// <param name="allowParallelExecution">Indicates if the step can be executed in parallel with others.</param>
    Task AddStep(IFlowStep step, string name, IEnumerable<string>? dependencies = null, Func<FlowContext, bool>? condition = null, bool allowParallelExecution = false);
}
