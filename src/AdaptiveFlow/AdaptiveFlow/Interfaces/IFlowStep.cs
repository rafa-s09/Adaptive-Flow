namespace AdaptiveFlow;

/// <summary>
/// Defines an interface for a flow step that does not return a value.
/// Provides a method for executing the step asynchronously.
/// </summary>
public interface IFlowStep
{
    /// <summary>
    /// Executes the flow step asynchronously.
    /// </summary>
    /// <param name="context">The flow context containing data for the execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe during execution. Defaults to none.</param>
    /// <returns>
    /// A task representing the asynchronous execution operation.
    /// </returns>
    Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}

