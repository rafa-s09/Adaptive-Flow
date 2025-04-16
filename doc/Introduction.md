# Introduction to AdaptiveFlow

**AdaptiveFlow** is a robust, reusable library and design pattern tailored for orchestrating asynchronous workflows in .NET. Its goal is to simplify the configuration and execution of sequences of steps while supporting advanced features like **dependencies**, **parallelism**, **dynamic configuration via JSON**, and **optional logging**. Ideal for applications requiring high-performance and adaptable pipelines, AdaptiveFlow excels in scenarios where flexibility and testability are paramount.

## How it Works

AdaptiveFlow organizes tasks in a modular and flexible pipeline, built upon the following key components:

1. **Steps:**
   - Represented by the `IFlowStep` interface for operations with no return value.
   - Each step is a self-contained logical unit, operating within a `FlowContext`—a shared container for data.
   - Example: One step validates input data, while another transforms it into a new format.

2. **FlowConfiguration:**
   - Defines the workflow sequence, specifying dependencies, execution conditions, and parallelism.
   - Supports configuration either programmatically or dynamically using JSON, enabling runtime adaptability.

3. **FlowManager:**
   - Manages workflow execution, ensuring dependencies are respected while balancing concurrency and parallel execution.
   - Utilizes `Channel<FlowContext>` for queuing and asynchronously processing contexts, with configurable resource limits.
   - Extensible with custom processors via the `IChannelProcessor` interface and supports optional logging.

4. **FlowContext:**
   - A thread-safe key-value dictionary that facilitates data sharing and communication between steps.
   - Supports scoped storage for step-specific or workflow-wide data.

---

### Example 1: Programmatic Workflow Configuration Without JSON

#### Programmatic Configuration:
```csharp
var flowConfig = new FlowConfiguration()
    .AddStep(new DataValidationStep(), "Validation")
    .AddStep(new DataTransformationStep(), "Transformation", dependsOn: new[] { "Validation" }, isParallel: true)
    .AddStep(new SaveToDatabaseStep(), "Save", dependsOn: new[] { "Transformation" });

var flowManager = new FlowManager(flowConfig);
await flowManager.StartProcessingAsync();
```

---

### Example 2: A Basic Workflow Configured Using JSON

#### JSON Configuration:
```csharp
[
  {
    "StepType": "DataValidationStep",
    "StepName": "Validation",
    "IsParallel": false
  },
  {
    "StepType": "DataTransformationStep",
    "StepName": "Transformation",
    "IsParallel": true,
    "DependsOn": ["Validation"]
  },
  {
    "StepType": "SaveToDatabaseStep",
    "StepName": "Save",
    "DependsOn": ["Transformation"]
  }
]
```

#### Workflow Execution:
```csharp
var stepRegistry = new Dictionary<string, Type>
{
    { "DataValidationStep", typeof(DataValidationStep) },
    { "DataTransformationStep", typeof(DataTransformationStep) },
    { "SaveToDatabaseStep", typeof(SaveToDatabaseStep) }
};

var serviceProvider = new ServiceCollection()
    .AddSingleton<DataValidationStep>()
    .AddSingleton<DataTransformationStep>()
    .AddSingleton<SaveToDatabaseStep>()
    .BuildServiceProvider();

var jsonConfig = File.ReadAllText("workflow.json");
var flowConfig = FlowConfiguration.FromJson(jsonConfig, serviceProvider, stepRegistry);

var flowManager = new FlowManager(flowConfig);
await flowManager.StartProcessingAsync();
```

---

## Benefits

- **Dynamic Configuration:** Adapt workflows at runtime using JSON, making updates seamless without recompilation.
- **Concurrency & Scalability:** Built-in support for parallel execution and configurable limits optimize performance.
- **Flexibility:** Decoupled and modular design supports custom components and behavior.
- **Ease of Testing:** Interfaces like `IChannelProcessor` enable mock-based testing for better reliability.
- **Security by Design:** Type registration ensures that only trusted, explicitly defined components are used dynamically.

---

## Use Cases

**AdaptiveFlow** shines in scenarios requiring both adaptability and precision. Common applications include:
- **Batch Data Processing:** Transform large datasets with multiple interdependent steps.
- **API Request Validation:** Validate and transform incoming requests through flexible pipelines.
- **ETL Pipelines:** Extract, Transform, and Load data in complex workflows.
- **Continuous Integration (CI) Pipelines:** Orchestrate build, test, and deployment steps in a CI/CD setup.


