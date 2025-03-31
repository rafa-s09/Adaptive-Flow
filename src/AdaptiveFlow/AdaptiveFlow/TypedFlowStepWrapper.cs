namespace AdaptiveFlow;

/// <summary>
/// Wraps an <see cref="IFlowStep{TResponse}"/> implementation to conform to the <see cref="IFlowStepWrapper"/> interface.
/// This class enables the integration of typed flow steps (those returning a specific <typeparamref name="TResponse"/>)
/// into the <see cref="FlowManager"/> and <see cref="FlowConfiguration"/>, allowing the execution of steps with return values
/// alongside void steps in a unified flow pipeline. The wrapper converts the typed result into a generic <see cref="object"/>
/// to be collected and returned as part of the flow's <see cref="FlowResult"/>.
/// <br/><br/>
/// Example:
/// <code>
/// public class ComputeStep : IFlowStep&lt;int&gt;
/// {
///     public async Task&lt;int&gt; ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         return 42;
///     }
/// }
/// 
/// var wrapper = new TypedFlowStepWrapper&lt;int&gt;(new ComputeStep());
/// var context = new FlowContext();
/// var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
/// Console.WriteLine(result); // Outputs: 42
/// </code>
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the wrapped <see cref="IFlowStep{TResponse}"/> implementation.</typeparam>
public class TypedFlowStepWrapper<TResponse> : IFlowStepWrapper
{
    private readonly IFlowStep<TResponse> _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedFlowStepWrapper{TResponse}"/> class with the specified typed flow step.
    /// </summary>
    /// <param name="step">The <see cref="IFlowStep{TResponse}"/> instance to wrap. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="step"/> is null.</exception>
    public TypedFlowStepWrapper(IFlowStep<TResponse> step)
    {
        _step = step ?? throw new ArgumentNullException(nameof(step), "The flow step cannot be null.");
    }

    /// <summary>
    /// Executes the wrapped <see cref="IFlowStep{TResponse}"/> asynchronously and returns its result as an <see cref="object"/>.
    /// This method bridges the typed step's execution to the generic <see cref="IFlowStepWrapper"/> interface, allowing
    /// the <see cref="FlowManager"/> to collect and include the result in the flow's <see cref="FlowResult"/>.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// var wrapper = new TypedFlowStepWrapper&lt;string&gt;(new GreetingStep());
    /// var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
    /// Console.WriteLine(result); // Outputs: "Hello, World!"
    /// </code>
    /// </summary>
    /// <param name="context">The execution context containing shared data for the flow step.</param>
    /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
    /// <returns>A task yielding the result of the wrapped step as an <see cref="object"/>, or null if no result is applicable.</returns>
    /// <exception cref="Exception">Thrown if the wrapped step encounters an error during execution.</exception>
    public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        return await _step.ExecuteAsync(context, cancellationToken);
    }
}