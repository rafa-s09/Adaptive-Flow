namespace AdaptiveFlow;

/// <summary>
/// Represents a flow execution context that stores shared data in a thread-safe manner.
/// This class is designed to hold arbitrary key-value pairs that can be accessed or modified by flow steps,
/// particularly in concurrent or parallel execution scenarios.
/// <br/><br/>
/// Example:
/// <code>
/// var context = new FlowContext();
/// context.Data.TryAdd("UserId", 123);
/// Console.WriteLine(context.Data["UserId"]); // Outputs: 123
/// </code>
/// </summary>
public class FlowContext
{
    /// <summary>
    /// A thread-safe dictionary that stores key-value pairs shared across flow steps.
    /// Keys are strings, and values can be any object, allowing flexible data exchange during execution.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// context.Data["Input"] = "Hello";
    /// if (context.Data.TryGetValue("Input", out var value))
    ///     Console.WriteLine(value); // Outputs: Hello
    /// </code>
    /// </summary>
    public ConcurrentDictionary<string, object> Data { get; } = new();
}

/// <summary>
/// Represents a strongly-typed flow execution context for scenarios requiring a specific data model.
/// This allows type-safe access to the context data, avoiding the need for casting.
/// <br/><br/>
/// Example:
/// <code>
/// var context = new FlowContext&lt;string&gt; { Data = "Processing" };
/// Console.WriteLine(context.Data); // Outputs: Processing
/// </code>
/// </summary>
/// <typeparam name="TModel">The type of data stored in the context.</typeparam>
public class FlowContext<TModel>
{
    /// <summary>
    /// The strongly-typed data stored in the context, accessible directly without casting.
    /// This property holds the data model specific to the flow, providing type safety and simplicity.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext&lt;int&gt; { Data = 42 };
    /// context.Data += 8;
    /// Console.WriteLine(context.Data); // Outputs: 50
    /// </code>
    /// </summary>
    public TModel? Data { get; set; }
}

