# Class `FlowContext`

Represents a flow execution context that stores shared data in a thread-safe manner. This class is designed to hold arbitrary key-value pairs that can be accessed or modified by flow steps, particularly in concurrent or parallel execution scenarios.

## Example

```csharp
var context = new FlowContext();
context.Data.TryAdd("UserId", 123);
Console.WriteLine(context.Data["UserId"]); // Outputs: 123
```

## Properties

### Data

```csharp
public ConcurrentDictionary<string, object> Data { get; } = new();
```

A thread-safe dictionary that stores key-value pairs shared across flow steps. Keys are strings, and values can be any object, allowing flexible data exchange during execution.

#### Example

```csharp
var context = new FlowContext();
context.Data["Input"] = "Hello";
if (context.Data.TryGetValue("Input", out var value))
    Console.WriteLine(value); // Outputs: Hello
```
<br/><br/>

---

# Class `FlowContext<TModel>`

Represents a strongly-typed flow execution context for scenarios requiring a specific data model. This allows type-safe access to the context data, avoiding the need for casting.

## Type Parameters
- **`TModel`**: The type of data stored in the context.

## Example

```csharp
var context = new FlowContext<string> { Data = "Processing" };
Console.WriteLine(context.Data); // Outputs: Processing
```

## Properties

### Data

```csharp
public TModel? Data { get; set; }
```

The strongly-typed data stored in the context, accessible directly without casting. This property holds the data model specific to the flow, providing type safety and simplicity.

#### Example

```csharp
var context = new FlowContext<int> { Data = 42 };
context.Data += 8;
Console.WriteLine(context.Data); // Outputs: 50
```