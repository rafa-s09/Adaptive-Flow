namespace AdaptiveFlow;

public class FlowConfiguration
{
    internal List<(IFlowStepWrapper, string, Func<FlowContext, bool>, bool, string[])> Steps { get; } = [];

    public FlowConfiguration AddStep(IFlowStep step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.Add((new VoidFlowStepWrapper(step), stepName, condition ?? (_ => true), isParallel, dependsOn ?? []));
        return this;
    }

    public FlowConfiguration AddSteps(IEnumerable<(IFlowStepWrapper Step, string StepName)> steps, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.AddRange(steps.Select(s => (s.Step, s.StepName, condition ?? (_ => true), isParallel, dependsOn ?? [])));
        return this;
    }

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

    public List<(IFlowStepWrapper, string, Func<FlowContext, bool>, bool, string[])> GetSteps() => Steps;

    private record StepConfig(string StepType, string StepName, bool IsParallel = false, string[]? DependsOn = null);
}