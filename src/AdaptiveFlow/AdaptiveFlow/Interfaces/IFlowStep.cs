namespace AdaptiveFlow.Interfaces;

/// <summary>
/// Represents a step in a flow that can be executed asynchronously.
/// </summary>
public interface IFlowStep
{
    /// <summary>
    /// Executes the step asynchronously, providing access to the FlowContext.
    /// </summary>
    /// <param name="context">The FlowContext containing shared data between steps.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. The default value is CancellationToken.None.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the step.
    /// May return an object or null, depending on the step's implementation.
    /// </returns>
    Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}