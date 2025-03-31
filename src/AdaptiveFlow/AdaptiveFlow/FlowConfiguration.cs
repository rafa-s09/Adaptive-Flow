namespace AdaptiveFlow;

/// <summary>
/// Configures a sequence of steps for a flow execution, including conditions and parallel execution options.
/// This class allows chaining configuration calls to build a flow pipeline.
/// <br/><br/>
/// Example:
/// <code>
/// var config = new FlowConfiguration()
///     .AddStep(new LogStep(), context => context.Data.ContainsKey("UserId"))
///     .AddSteps(new[] { new Step1(), new Step2() }, isParallel: true);
/// </code>
/// </summary>
public class FlowConfiguration
{
    public List<(IFlowStep Step, Func<FlowContext, bool> Condition, bool IsParallel)> Steps { get; } = new();

    /// <summary>
    /// Adds a single step to the flow configuration with an optional execution condition and parallel flag.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// config.AddStep(new LogStep(), context => (int)context.Data["UserId"] > 0, isParallel: false);
    /// </code>
    /// </summary>
    /// <param name="step">The flow step to add to the configuration.</param>
    /// <param name="condition">An optional predicate that determines if the step should execute. Defaults to always true if null.</param>
    /// <param name="isParallel">Indicates whether the step should run in parallel with other parallel steps. Defaults to false (sequential).</param>
    /// <returns>The current FlowConfiguration instance for method chaining.</returns>
    public FlowConfiguration AddStep(IFlowStep step, Func<FlowContext, bool>? condition = null, bool isParallel = false)
    {
        Steps.Add((step, condition ?? (_ => true), isParallel));
        return this;
    }

    /// <summary>
    /// Adds multiple steps to the flow configuration with a shared condition and parallel execution option.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// config.AddSteps(new[] { new Step1(), new Step2() }, context => context.Data["Enabled"] == true, isParallel: true);
    /// </code>
    /// </summary>
    /// <param name="steps">The collection of flow steps to add.</param>
    /// <param name="condition">An optional predicate applied to all steps. Defaults to always true if null.</param>
    /// <param name="isParallel">Indicates whether the steps should run in parallel. Defaults to false (sequential).</param>
    /// <returns>The current FlowConfiguration instance for method chaining.</returns>
    public FlowConfiguration AddSteps(IEnumerable<IFlowStep> steps, Func<FlowContext, bool>? condition = null, bool isParallel = false)
    {
        Steps.AddRange(steps.Select(step => (step, condition ?? (_ => true), isParallel)));
        return this;
    }
}
