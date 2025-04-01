# Class `FlowConfiguration`

Configures a sequence of steps for a flow execution, including conditions, parallel execution options, and dependencies. This class allows chaining configuration calls to build a flow pipeline with both void and typed steps.

## Example

```csharp
var config = new FlowConfiguration()
    .AddStep(new LogStep(), context => context.Data.ContainsKey("UserId"))
    .AddStep(new ComputeStep(), dependsOn: new[] { "LogStep" }, isParallel: true);
```

## Properties

### Steps

```csharp
public List<(IFlowStepWrapper Step, string StepName, Func<FlowContext, bool> Condition, bool IsParallel, string[] DependsOn)> Steps { get; } = [];
```

A list of tuples representing the configured flow steps and their execution metadata. Each tuple contains the step wrapper, its unique name, an optional condition for execution, a flag for parallel execution, and an array of step names it depends on. This collection defines the order and behavior of the flow pipeline.

#### Example

```csharp
var config = new FlowConfiguration();
config.Steps.Add((new VoidFlowStepWrapper(new LogStep()), "Log", _ => true, false, Array.Empty<string>()));
Console.WriteLine(config.Steps[0].StepName); // Outputs: Log
```

## Methods

### AddStep (Void Step)

```csharp
public FlowConfiguration AddStep(IFlowStep step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
```

Adds a void step to the flow configuration with an optional condition, parallel flag, and dependencies.

#### Parameters
- **`step`** (`IFlowStep`): The void step to add.
- **`stepName`** (`string`): A unique name for the step.
- **`condition`** (`Func<FlowContext, bool>?`): An optional condition to determine if the step should execute. Defaults to `null` (always true).
- **`isParallel`** (`bool`): Indicates if the step should run in parallel. Defaults to `false`.
- **`dependsOn`** (`string[]?`): An optional array of step names this step depends on. Defaults to `null`.

#### Returns
- `FlowConfiguration`: The current instance for method chaining.

#### Example

```csharp
config.AddStep(new LogStep(), "LogStep", context => (int)context.Data["UserId"] > 0);
```

---

### AddStep (Typed Step)

```csharp
public FlowConfiguration AddStep<TResponse>(IFlowStep<TResponse> step, string stepName, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
```

Adds a typed step to the flow configuration with an optional condition, parallel flag, and dependencies.

#### Type Parameters
- **`TResponse`**: The type of response returned by the step.

#### Parameters
- **`step`** (`IFlowStep<TResponse>`): The typed step to add.
- **`stepName`** (`string`): A unique name for the step.
- **`condition`** (`Func<FlowContext, bool>?`): An optional condition to determine if the step should execute. Defaults to `null` (always true).
- **`isParallel`** (`bool`): Indicates if the step should run in parallel. Defaults to `false`.
- **`dependsOn`** (`string[]?`): An optional array of step names this step depends on. Defaults to `null`.

#### Returns
- `FlowConfiguration`: The current instance for method chaining.

#### Example

```csharp
config.AddStep(new ComputeStep(), "ComputeStep", dependsOn: new[] { "LogStep" }, isParallel: true);
```

---

### AddSteps

```csharp
public FlowConfiguration AddSteps(IEnumerable<(IFlowStepWrapper Step, string StepName)> steps, Func<FlowContext, bool>? condition = null, bool isParallel = false, string[]? dependsOn = null)
```

Adds multiple steps to the flow configuration with a shared condition, parallel execution option, and dependencies.

#### Parameters
- **`steps`** (`IEnumerable<(IFlowStepWrapper Step, string StepName)>`): A collection of tuples containing step wrappers and their names.
- **`condition`** (`Func<FlowContext, bool>?`): An optional condition to determine if the steps should execute. Defaults to `null` (always true).
- **`isParallel`** (`bool`): Indicates if the steps should run in parallel. Defaults to `false`.
- **`dependsOn`** (`string[]?`): An optional array of step names these steps depend on. Defaults to `null`.

#### Returns
- `FlowConfiguration`: The current instance for method chaining.

#### Example

```csharp
config.AddSteps(new (IFlowStepWrapper, string)[] 
{ 
    (new VoidFlowStepWrapper(new LogStep()), "LogStep"), 
    (new TypedFlowStepWrapper<int>(new ComputeStep()), "ComputeStep") 
}, dependsOn: new[] { "InitialStep" });
```

---

### FromJson

```csharp
public static FlowConfiguration FromJson(string json, IServiceProvider serviceProvider, IReadOnlyDictionary<string, Type> stepRegistry)
```

Creates a `FlowConfiguration` instance from a JSON string, dynamically instantiating steps using a service provider and a client-provided step registry. This method ensures security by only allowing step types explicitly registered in the `stepRegistry`, preventing unauthorized type loading. Designed for use in a reusable library, requiring clients to supply their own step mappings.

#### Parameters
- **`json`** (`string`): A JSON string representing the flow steps, with each step specifying a `StepType` matching a key in the registry.
- **`serviceProvider`** (`IServiceProvider`): The service provider used to resolve step instances from the registered types.
- **`stepRegistry`** (`IReadOnlyDictionary<string, Type>`): A dictionary mapping step type identifiers to their corresponding `Type` implementations, provided by the client.

#### Returns
- `FlowConfiguration`: A configured `FlowConfiguration` instance.

#### Exceptions
- `ArgumentNullException`: Thrown if `json`, `serviceProvider`, or `stepRegistry` is null.
- `ArgumentException`: Thrown if the JSON is invalid, a step type is not registered, or resolution fails.

#### Example

```csharp
var json = @"[
    {'StepType': 'ValidateInput', 'StepName': 'Validate', 'IsParallel': false},
    {'StepType': 'Compute', 'StepName': 'Compute', 'IsParallel': true, 'DependsOn': ['Validate']}
]";
var stepRegistry = new Dictionary<string, Type>
{
    ["ValidateInput"] = typeof(ValidarEntradaStep),
    ["Compute"] = typeof(ComputeStep)
};
var config = FlowConfiguration.FromJson(json, serviceProvider, stepRegistry);
```

---

### AddTypedStep (Private)

```csharp
private static void AddTypedStep<TResponse>(FlowConfiguration config, object stepInstance, StepConfig stepConfig)
```

Adds a typed flow step (`IFlowStep<TResponse>`) to the specified `FlowConfiguration` instance. This helper method ensures that the provided step instance matches the expected generic type `TResponse`, facilitating dynamic configuration of typed steps from sources like JSON within the `FromJson` method.

#### Type Parameters
- **`TResponse`**: The type of response returned by the typed flow step.

#### Parameters
- **`config`** (`FlowConfiguration`): The `FlowConfiguration` instance to which the step will be added.
- **`stepInstance`** (`object`): The step instance to add, expected to implement `IFlowStep<TResponse>`.
- **`stepConfig`** (`StepConfig`): The configuration details for the step, including its name, parallelism, and dependencies.

#### Exceptions
- `ArgumentException`: Thrown if `stepInstance` does not implement `IFlowStep<TResponse>` with the specified `TResponse`.

#### Example

```csharp
var config = new FlowConfiguration();
var stepConfig = new StepConfig("Compute", "Compute", true, null);
var stepInstance = new ComputeStep(); // Implements IFlowStep<int>
FlowConfiguration.AddTypedStep<int>(config, stepInstance, stepConfig);
```

---

### StepConfig (Private Record)

```csharp
private record StepConfig(string StepType, string StepName, bool IsParallel = false, string[]? DependsOn = null);
```

Represents the configuration for a single flow step as parsed from a dynamic source, such as JSON. This record defines the essential properties of a step, including its type, name, parallelism setting, and optional dependencies, used by `FlowConfiguration.FromJson` to instantiate and configure steps dynamically.

#### Parameters
- **`StepType`** (`string`): The identifier of the step type as registered in the step registry (e.g., "LogStep").
- **`StepName`** (`string`): A unique name identifying the step within the flow configuration.
- **`IsParallel`** (`bool`): Indicates whether the step should be executed in parallel with other parallel steps. Defaults to `false`.
- **`DependsOn`** (`string[]?`): An optional array of step names that this step depends on, ensuring they execute first. Defaults to `null`.

#### Example

```csharp
var json = @"[{'StepType': 'LogStep', 'StepName': 'Log', 'IsParallel': false, 'DependsOn': null}]";
var stepConfigs = JsonSerializer.Deserialize<List<StepConfig>>(json);
Console.WriteLine(stepConfigs[0].StepName); // Outputs: Log
```