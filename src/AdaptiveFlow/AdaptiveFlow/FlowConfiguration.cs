namespace AdaptiveFlow;

/// <summary>
/// Configures a sequence of steps for a flow execution, including conditions, parallel execution options, and dependencies.
/// This class allows chaining configuration calls to build a flow pipeline with both void and typed steps.
/// <br/><br/>
/// Example:
/// <code>
/// var config = new FlowConfiguration()
///     .AddStep(new LogStep(), context => context.Data.ContainsKey("UserId"))
///     .AddStep(new ComputeStep(), dependsOn: new[] { "LogStep" }, isParallel: true);
/// </code>
/// </summary>
public class FlowConfiguration
{

    /// <summary>
    /// A list of tuples representing the configured flow steps and their execution metadata.
    /// Each tuple contains the step wrapper, its unique name, an optional condition for execution,
    /// a flag for parallel execution, and an array of step names it depends on.
    /// This collection defines the order and behavior of the flow pipeline.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var config = new FlowConfiguration();
    /// config.Steps.Add((new VoidFlowStepWrapper(new LogStep()), "Log", _ =&gt; true, false, Array.Empty&lt;string&gt;()));
    /// Console.WriteLine(config.Steps[0].StepName); // Outputs: Log
    /// </code>
    /// </summary>
    public List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> Steps { get; } = [];

    /// <summary>
    /// Adds a void step to the flow configuration with an optional condition, parallel flag, and dependencies.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// config.AddStep(new LogStep(), "LogStep", context =&gt; (int)context.Data["UserId"] &gt; 0);
    /// </code>
    /// </summary>
    public FlowConfiguration AddStep(IFlowStep step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.Add((new VoidFlowStepWrapper(step), stepName, condition ?? (_ => true), isParallel, dependsOn ?? []));
        return this;
    }

    /// <summary>
    /// Adds a typed step to the flow configuration with an optional condition, parallel flag, and dependencies.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// config.AddStep(new ComputeStep(), "ComputeStep", dependsOn: new[] { "LogStep" }, isParallel: true);
    /// </code>
    /// </summary>
    public FlowConfiguration AddStep<TResponse>(IFlowStep<TResponse> step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.Add((new TypedFlowStepWrapper<TResponse>(step), stepName, condition ?? (_ => true), isParallel, dependsOn ?? []));
        return this;
    }

    /// <summary>
    /// Adds multiple steps to the flow configuration with a shared condition, parallel execution option, and dependencies.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// config.AddSteps(new (IFlowStepWrapper, string)[] 
    /// { 
    ///     (new VoidFlowStepWrapper(new LogStep()), "LogStep"), 
    ///     (new TypedFlowStepWrapper&lt;int&gt;(new ComputeStep()), "ComputeStep") 
    /// }, dependsOn: new[] { "InitialStep" });
    /// </code>
    /// </summary>
    public FlowConfiguration AddSteps(IEnumerable<(IFlowStepWrapper Step, string StepName)> steps, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
    {
        Steps.AddRange(steps.Select(s => (s.Step, s.StepName, condition ?? (_ => true), isParallel, dependsOn ?? [])));
        return this;
    }

    /// <summary>
    /// Creates a <see cref="FlowConfiguration"/> instance from a JSON string, dynamically instantiating steps using a service provider and a client-provided step registry.
    /// This method ensures security by only allowing step types explicitly registered in the <paramref name="stepRegistry"/>, preventing unauthorized type loading.
    /// Designed for use in a reusable library, requiring clients to supply their own step mappings.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var json = @"[
    ///     {'StepType': 'ValidateInput', 'StepName': 'Validate', 'IsParallel': false},
    ///     {'StepType': 'Compute', 'StepName': 'Compute', 'IsParallel': true, 'DependsOn': ['Validate']}
    /// ]";
    /// var stepRegistry = new Dictionary&lt;string, Type&gt;
    /// {
    ///     ["ValidateInput"] = typeof(ValidarEntradaStep),
    ///     ["Compute"] = typeof(ComputeStep)
    /// };
    /// var config = FlowConfiguration.FromJson(json, serviceProvider, stepRegistry);
    /// </code>
    /// </summary>
    /// <param name="json">A JSON string representing the flow steps, with each step specifying a <see cref="StepConfig.StepType"/> matching a key in the registry.</param>
    /// <param name="serviceProvider">The service provider used to resolve step instances from the registered types.</param>
    /// <param name="stepRegistry">A dictionary mapping step type identifiers (e.g., "ValidateInput") to their corresponding <see cref="Type"/> implementations, provided by the client.</param>
    /// <returns>A configured <see cref="FlowConfiguration"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="json"/>, <paramref name="serviceProvider"/>, or <paramref name="stepRegistry"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the JSON is invalid, a step type is not registered, or resolution fails.</exception>
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
                Type? flowStepInterface = stepInstance.GetType().GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFlowStep<>));
                if (flowStepInterface != null)
                {
                    Type responseType = flowStepInterface.GetGenericArguments()[0];
                    MethodInfo? method = typeof(FlowConfiguration).GetMethod(nameof(AddTypedStep), BindingFlags.NonPublic | BindingFlags.Static);
                    MethodInfo? genericMethod = method?.MakeGenericMethod(responseType);
                    genericMethod?.Invoke(null, [config, stepInstance, stepConfig]);
                }
                else
                {
                    throw new ArgumentException($"Type '{stepConfig.StepType}' does not implement IFlowStep or IFlowStep<T>.", nameof(json));
                }
            }
        }

        return config;
    }

    /// <summary>
    /// Adds a typed flow step (<see cref="IFlowStep{TResponse}"/>) to the specified <see cref="FlowConfiguration"/> instance.
    /// This helper method ensures that the provided step instance matches the expected generic type <typeparamref name="TResponse"/>,
    /// facilitating dynamic configuration of typed steps from sources like JSON within the <see cref="FlowConfiguration.FromJson"/> method.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var config = new FlowConfiguration();
    /// var stepConfig = new StepConfig("Compute", "Compute", true, null);
    /// var stepInstance = new ComputeStep(); // Implements IFlowStep&lt;int&gt;
    /// FlowConfiguration.AddTypedStep&lt;int&gt;(config, stepInstance, stepConfig);
    /// </code>
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned by the typed flow step.</typeparam>
    /// <param name="config">The <see cref="FlowConfiguration"/> instance to which the step will be added.</param>
    /// <param name="stepInstance">The step instance to add, expected to implement <see cref="IFlowStep{TResponse}"/>.</param>
    /// <param name="stepConfig">The configuration details for the step, including its name, parallelism, and dependencies.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stepInstance"/> does not implement <see cref="IFlowStep{TResponse}"/> with the specified <typeparamref name="TResponse"/>.</exception>
    private static void AddTypedStep<TResponse>(FlowConfiguration config, object stepInstance, StepConfig stepConfig)
    {
        if (stepInstance is IFlowStep<TResponse> typedStep)
            config.AddStep(typedStep, stepConfig.StepName, isParallel: stepConfig.IsParallel, dependsOn: stepConfig.DependsOn);
        else
            throw new ArgumentException($"Step instance does not match expected IFlowStep<{typeof(TResponse).Name}> type.", nameof(stepInstance));
    }

    /// <summary>
    /// Represents the configuration for a single flow step as parsed from a dynamic source, such as JSON.
    /// This record defines the essential properties of a step, including its type, name, parallelism setting, and optional dependencies,
    /// used by <see cref="FlowConfiguration.FromJson"/> to instantiate and configure steps dynamically.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var json = @"[{'StepType': 'LogStep', 'StepName': 'Log', 'IsParallel': false, 'DependsOn': null}]";
    /// var stepConfigs = JsonSerializer.Deserialize&lt;List&lt;StepConfig&gt;&gt;(json);
    /// Console.WriteLine(stepConfigs[0].StepName); // Outputs: Log
    /// </code>
    /// </summary>
    /// <param name="StepType">The identifier of the step type as registered in the step registry (e.g., "LogStep").</param>
    /// <param name="StepName">A unique name identifying the step within the flow configuration.</param>
    /// <param name="IsParallel">Indicates whether the step should be executed in parallel with other parallel steps. Defaults to false.</param>
    /// <param name="DependsOn">An optional array of step names that this step depends on, ensuring they execute first. Defaults to null.</param>
    private record StepConfig(string StepType, string StepName, bool IsParallel = false, string[]? DependsOn = null);
}