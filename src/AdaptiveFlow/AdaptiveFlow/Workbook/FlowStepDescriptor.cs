namespace AdaptiveFlow.Workbook;

/// <summary>
/// Represents the metadata for a flow step, including its name, execution logic, dependencies, conditions, and execution settings.
/// </summary>
/// <param name="Name">The unique name of the step.</param>
/// <param name="Step">The actual step logic to be executed.</param>
/// <param name="Dependencies">A list of step names that must be completed before this step can execute.</param>
/// <param name="Condition">An optional condition that must be met for this step to execute.</param>
/// <param name="AllowParallelExecution">Indicates whether the step is allowed to execute in parallel with others.</param>
public record FlowStepDescriptor(FlowKey Name, IFlowStep Step, List<FlowKey> Dependencies, Func<FlowContext, bool>? Condition, bool AllowParallelExecution);