namespace AdaptiveFlow;

/// <summary>
/// Wraps an <see cref="IFlowStep"/> implementation to conform to the <see cref="IFlowStepWrapper"/> interface.
/// This class adapts void flow steps (those that do not return a value) for use within the <see cref="FlowManager"/> 
/// and <see cref="FlowConfiguration"/>, enabling seamless execution alongside typed steps in a unified flow pipeline.
/// Since <see cref="IFlowStep"/> does not produce a result, this wrapper returns <c>null</c> to maintain compatibility 
/// with the generic result collection mechanism of the <see cref="FlowResult"/>.
/// <br/><br/>
/// Example:
/// <code>
/// public class LogStep : IFlowStep
/// {
///     public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         Console.WriteLine("Logging...");
///         await Task.Delay(100, cancellationToken);
///     }
/// }
/// 
/// var wrapper = new VoidFlowStepWrapper(new LogStep());
/// var context = new FlowContext();
/// var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
/// Console.WriteLine(result == null); // Outputs: True
/// </code>
/// </summary>
public class VoidFlowStepWrapper : IFlowStepWrapper
{
    private readonly IFlowStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoidFlowStepWrapper"/> class with the specified void flow step.
    /// </summary>
    /// <param name="step">The <see cref="IFlowStep"/> instance to wrap. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="step"/> is null.</exception>
    public VoidFlowStepWrapper(IFlowStep step)
    {
        _step = step ?? throw new ArgumentNullException(nameof(step), "The flow step cannot be null.");
    }

    /// <summary>
    /// Executes the wrapped <see cref="IFlowStep"/> asynchronously and returns <c>null</c> as the result.
    /// This method ensures that void steps can be executed within the <see cref="FlowManager"/> pipeline,
    /// maintaining compatibility with the <see cref="IFlowStepWrapper"/> interface used to collect results.
    /// Since the wrapped step does not produce a value, <c>null</c> is returned to indicate no result.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// context.Data.TryAdd("Message", "Starting process");
    /// var wrapper = new VoidFlowStepWrapper(new LogStep());
    /// var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
    /// Console.WriteLine(result == null ? "No result" : result); // Outputs: No result
    /// </code>
    /// </summary>
    /// <param name="context">The execution context containing shared data for the flow step.</param>
    /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
    /// <returns>A task yielding <c>null</c>, as the wrapped step does not produce a result.</returns>
    /// <exception cref="Exception">Thrown if the wrapped step encounters an error during execution.</exception>
    public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await _step.ExecuteAsync(context, cancellationToken);
        return null;
    }
}