namespace AdaptiveFlow;

/// <summary>
/// Defines a wrapper interface for flow steps that return a value.
/// Provides a method for executing the step asynchronously.
/// </summary>
public interface IFlowStepWrapper
{
    /// <summary>
    /// Executes the flow step asynchronously and returns a result.
    /// </summary>
    /// <param name="context">The flow context containing data for the execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe during execution.</param>
    /// <returns>
    /// A task representing the asynchronous execution operation.
    /// The result is the output produced by the flow step or null if no output is generated.
    /// </returns>
    Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}