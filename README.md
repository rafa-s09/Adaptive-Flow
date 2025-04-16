# AdaptiveFlow

**AdaptiveFlow** is a robust, reusable library and design pattern tailored for orchestrating asynchronous workflows in .NET. Its goal is to simplify the configuration and execution of sequences of steps while supporting advanced features like **dependencies**, **parallelism**, **dynamic configuration via JSON**, and **optional logging**. Ideal for applications requiring high-performance and adaptable pipelines, AdaptiveFlow excels in scenarios where flexibility and testability are paramount.

[![NuGet Version](https://img.shields.io/nuget/v/AdaptiveFlow)](https://www.nuget.org/packages/AdaptiveFlow/)


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

---

# Contributions

Feel free to open issues or pull requests in the repository. Suggestions to improve the library are always welcome!

---

# MIT License

Copyright (c) 2025 Rafael Souza

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.