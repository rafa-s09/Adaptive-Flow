namespace AdaptiveFlow;

/// <summary>
/// A wrapper for flow steps that do not return a value.
/// Implements <see cref="IFlowStepWrapper"/> to provide a consistent interface for executing steps.
/// </summary>
public class VoidFlowStepWrapper : IFlowStepWrapper
{
    private readonly IFlowStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoidFlowStepWrapper"/> class.
    /// </summary>
    /// <param name="step">The flow step to be wrapped. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided step is null.</exception>
    public VoidFlowStepWrapper(IFlowStep step)
    {
        _step = step ?? throw new ArgumentNullException(nameof(step), "The flow step cannot be null.");
    }

    /// <summary>
    /// Executes the wrapped flow step asynchronously and returns null.
    /// </summary>
    /// <param name="context">The flow context containing data for the step execution.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// Always returns null since the wrapped step does not produce a result.
    /// </returns>
    public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await _step.ExecuteAsync(context, cancellationToken);
        return null;
    }
}