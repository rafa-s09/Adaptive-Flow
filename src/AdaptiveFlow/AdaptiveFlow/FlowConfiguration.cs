namespace AdaptiveFlow;

/// <summary>
/// Represents a configuration for defining a sequence of flow steps to be executed.
/// Provides support for conditions, dependencies, and parallel execution.
/// </summary>
public class FlowConfiguration
{
    /// <summary>
    /// A list of steps to be executed as part of the flow, including their metadata such as
    /// name, condition, parallel execution flag, and dependencies.
    /// </summary>
    internal List<(IFlowStepWrapper, string, Func<FlowContext, bool>, bool, string[])> Steps { get; } = [];

    /// <summary>
    /// Adds a single step to the configuration.
    /// </summary>
    /// <param name="step">The flow step to be added.</param>
    /// <param name="stepName">The name of the step. Must be unique within the flow.</param>
    /// <param name="condition">An optional condition that determines whether the step should be executed. Defaults to always true.</param>
    /// <param name="isParallel">Indicates if the step can be executed in parallel with others. Defaults to false.</param>
    /// <param name="dependsOn">An optional list of step names that this step depends on. Defaults to no dependencies.</param>
    /// <returns>The current <see cref="FlowConfiguration"/> instance for method chaining.</returns>
    public FlowConfiguration AddStep(IFlowStep step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.Add((new VoidFlowStepWrapper(step), stepName, condition ?? (_ => true), isParallel, dependsOn ?? []));
        return this;
    }

    /// <summary>
    /// Adds multiple steps to the configuration.
    /// </summary>
    /// <param name="steps">The collection of steps and their respective names.</param>
    /// <param name="condition">An optional condition that applies to all the added steps. Defaults to always true.</param>
    /// <param name="isParallel">Indicates if the steps can be executed in parallel. Defaults to false.</param>
    /// <param name="dependsOn">An optional list of step names that these steps depend on. Defaults to no dependencies.</param>
    /// <returns>The current <see cref="FlowConfiguration"/> instance for method chaining.</returns>
    public FlowConfiguration AddSteps(IEnumerable<(IFlowStepWrapper Step, string StepName)> steps, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.AddRange(steps.Select(s => (s.Step, s.StepName, condition ?? (_ => true), isParallel, dependsOn ?? [])));
        return this;
    }

    /// <summary>
    /// Creates a flow configuration from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string representing the flow configuration.</param>
    /// <param name="serviceProvider">The service provider used to resolve dependencies for the steps.</param>
    /// <param name="stepRegistry">A dictionary mapping step type names to their corresponding .NET <see cref="Type"/>.</param>
    /// <returns>A new <see cref="FlowConfiguration"/> instance based on the parsed JSON.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    /// <exception cref="ArgumentException">Thrown if the JSON is invalid or references unregistered step types.</exception>
    public static FlowConfiguration FromJson(string json, IServiceProvider serviceProvider, IReadOnlyDictionary<string, Type> stepRegistry)
    {
        if (json == null) throw new ArgumentNullException(nameof(json), "JSON configuration cannot be null.");
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");
        if (stepRegistry == null) throw new ArgumentNullException(nameof(stepRegistry), "Step registry cannot be null.");

        FlowConfiguration config = new();
        List<StepConfig> stepConfigs = JsonSerializer.Deserialize<List<StepConfig>>(json) ?? throw new ArgumentException("Invalid JSON configuration.", nameof(json));

        foreach (var stepConfig in stepConfigs)
        {
            if (!stepRegistry.TryGetValue(stepConfig.StepType, out Type? stepType))
                throw new ArgumentException($"Step type '{stepConfig.StepType}' is not registered in the step registry.", nameof(json));

            object stepInstance = serviceProvider.GetService(stepType) ?? throw new ArgumentException($"Could not resolve instance of '{stepConfig.StepType}' from the service provider.", nameof(json));

            if (stepInstance is IFlowStep voidStep)
            {
                config.AddStep(voidStep, stepConfig.StepName, isParallel: stepConfig.IsParallel, dependsOn: stepConfig.DependsOn);
            }
            else
            {
                throw new ArgumentException($"Type '{stepConfig.StepType}' does not implement IFlowStep or IFlowStep<T>.", nameof(json));
            }
        }

        return config;
    }

    /// <summary>
    /// Retrieves the list of steps in the configuration, including their metadata.
    /// </summary>
    /// <returns>A list of tuples representing the steps and their associated properties.</returns>
    public List<(IFlowStepWrapper, string, Func<FlowContext, bool>, bool, string[])> GetSteps() => Steps;

    /// <summary>
    /// Represents the structure of a step configuration loaded from JSON.
    /// </summary>
    /// <param name="StepType">The fully qualified type name of the step.</param>
    /// <param name="StepName">The unique name of the step.</param>
    /// <param name="IsParallel">Indicates if the step can be executed in parallel with others.</param>
    /// <param name="DependsOn">A list of step names that this step depends on.</param>
    private record StepConfig(string StepType, string StepName, bool IsParallel = false, string[]? DependsOn = null);
}