namespace AdaptiveFlow;

/// <summary>
/// Defines a step in a flow that can be executed asynchronously.
/// Implementations should perform a specific task within the flow and respect cancellation requests.
/// If an error occurs, implementations should throw an exception to be handled by the flow manager.
/// <br/><br/>
/// Example:
/// <code>
/// public class LogStep : IFlowStep
/// {
///     public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         cancellationToken.ThrowIfCancellationRequested();
///         if (!context.Data.ContainsKey("Email")) throw new ArgumentException("Email is required");
///         Console.WriteLine("Logging...");
///         await Task.Delay(100, cancellationToken);
///     }
/// }
/// </code>
/// </summary>
public interface IFlowStep
{
    /// <summary>
    /// Executes the flow step asynchronously using the provided context.
    /// </summary>
    /// <param name="context">The execution context containing shared data.</param>
    /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
    /// <returns>A task representing the asynchronous execution of the step.</returns>
    /// <exception cref="Exception">Thrown if the step encounters an error during execution.</exception>
    Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a step in a flow that returns a typed response upon execution.
/// Useful for steps that produce a result to be consumed by subsequent steps or the caller.
/// Implementations should throw exceptions if errors ocorrem during execution, which will be captured by the flow manager.
/// <br/><br/>
/// Example:
/// <code>
/// public class ComputeStep : IFlowStep&lt;int&gt;
/// {
///     public async Task&lt;int&gt; ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         cancellationToken.ThrowIfCancellationRequested();
///         if (!context.Data.ContainsKey("Input")) throw new ArgumentException("Input is required");
///         await Task.Delay(50, cancellationToken);
///         return 42;
///     }
/// }
/// </code>
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the step.</typeparam>
public interface IFlowStep<TResponse>
{
    /// <summary>
    /// Executes the flow step asynchronously and returns a result of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="context">The execution context containing shared data.</param>
    /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
    /// <returns>A task yielding a result of type <typeparamref name="TResponse"/> upon successful execution.</returns>
    /// <exception cref="Exception">Thrown if the step encounters an error during execution.</exception>
    Task<TResponse> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}