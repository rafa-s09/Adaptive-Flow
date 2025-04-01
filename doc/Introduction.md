# Introduction to Adaptive Flow

***AdaptiveFlow*** is a reusable library and a design pattern for orchestrating asynchronous workflows in .NET. It allows configuring and executing sequences of steps with support for dependencies, parallel execution, optional logging, high performance, and dynamic configuration via ***JSON***. Ideal for applications that require flexible, testable pipelines adaptable to different contexts.

## The AdaptiveFlow Pattern

***AdaptiveFlow*** is more than just a library — it is a pattern for building adaptable workflows. It addresses common challenges in systems that require step-based processing, such as validation, transformation, and data persistence, by offering a modular and extensible approach.

### How It Works

The ***AdaptiveFlow*** pattern organizes task execution into a structured yet flexible pipeline, based on the following components:

1. **Steps:**
    - Represented by `IFlowStep` (no return value) or `IFlowStep<TResponse>` (with return value).
    - Each step is an independent logic unit operating on a `FlowContext` (a shared data container).
    - Example: One step may validate input, while another encrypts data.

2. **Configuration (FlowConfiguration):**
    - Defines the sequence of steps, their execution conditions, dependencies, and whether they should run in parallel.
    - Supports both programmatic and dynamic configuration via ***JSON***, allowing adjustments at design or runtime.

3. **Manager (FlowManager):**
    - Orchestrates step execution, respecting dependencies while managing concurrency and parallelism.
    - Uses a channel (`Channel<FlowContext>`) to queue and process contexts asynchronously, with configurable limits for high loads.
    - Supports optional logging and allows channel processing to be replaced via `IChannelProcessor`.

4. **Context (FlowContext):**
    - A key-value dictionary that carries data between steps, enabling smooth communication and shared state.

## Pattern Principles

- **Modularity:** Steps are isolated and interchangeable, facilitating reuse and testing.
- **Flexibility:** Dependencies and parallelism are configurable, adapting the workflow to different scenarios.
- **Scalability:** Support for concurrency and channel limits ensures high-performance processing.
- **Security:** Dynamic configuration requires explicit registration of allowed types, preventing arbitrary execution.
- **Testability:** Abstracted components (such as `IChannelProcessor`) enable simulation in tests.

## Execution Flow

1. The client defines the steps and their configurations in a `FlowConfiguration`.
2. The `FlowManager` receives the configuration and initializes a channel to queue contexts.
3. Each context is processed, executing steps in an order determined by dependencies:
    - Sequential steps run one after another.
    - Parallel steps run simultaneously, respecting `maxParallelism`.
4. Results are collected in `FlowResult`, including success/failure status and step outputs.

## Benefits

- **Adaptability:** Adjust workflows dynamically without recompiling code.
- **Maintainability:** Isolated steps simplify debugging and system evolution.
- **Reusability:** Apply the same pattern across different parts of a project or in separate projects.