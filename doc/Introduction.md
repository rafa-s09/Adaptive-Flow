# Introduction to Adaptive Flow

**AdaptiveFlow** is a reusable library and design pattern for orchestrating asynchronous workflows in .NET. It allows you to configure and execute sequences of steps with support for dependencies, parallel execution, optional logging, high performance and dynamic configuration via JSON. It is ideal for applications that require flexible and testable pipelines, adaptable to different contexts.

## How it Works

**AdaptiveFlow** organizes the execution of tasks in a structured but flexible pipeline, based on four main components:

1. **Steps:**
- Defined by the `IFlowStep` (no return) or `IFlowStep<TResponse>` (typed return) interfaces.
- Each step is an independent unit of logic that operates on a `FlowContext`, a shared data container.
- Example: One step can validate input data, while another performs a transformation.

2. **FlowConfiguration:**
- Defines the sequence of steps, execution conditions, dependencies, and whether they should be executed in parallel.
- Can be configured programmatically or dynamically via JSON, allowing adjustments at design or runtime.

3. **FlowManager:**
- Orchestrates the execution of steps, respecting dependencies and managing concurrency and parallelism.
- Uses a channel (`Channel<FlowContext>`) to queue and process contexts asynchronously, with configurable limits to support high load.
- Supports optional logging and allows overriding channel processing via `IChannelProcessor`.

4. **FlowContext:**
- A thread-safe key-value dictionary that transports data between steps, facilitating communication and shared state.

<br/>

![AdaptiveFlow Diagram](Resources/FlowDiagram.png)

## Benefits

- **Flexibility:** Dynamic configuration via JSON or code allows adjustments without recompilation.
- **Scalability:** Support for concurrency and channel limits ensures performance under high load.
- **Testability:** Abstracted components such as `IChannelProcessor` facilitate simulations in tests.
- **Security:** Dynamic configuration requires explicit registration of allowed types.

## Use Cases

- Batch data processing.
- Validation and transformation of inputs in APIs.
- Continuous integration or ETL (Extract, Transform, Load) pipelines.

**AdaptiveFlow** is a powerful solution for creating robust and adaptive workflows in .NET, combining simplicity with high performance.