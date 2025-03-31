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
    public TModel? Data { get; set; }
}

