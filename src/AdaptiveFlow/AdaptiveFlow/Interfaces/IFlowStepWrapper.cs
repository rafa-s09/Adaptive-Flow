namespace AdaptiveFlow;

/// <summary>
/// Defines a generic wrapper interface for flow steps, enabling the execution of both void steps (<see cref="IFlowStep"/>)
/// and typed response steps (<see cref="IFlowStep{TResponse}"/>) within the <see cref="FlowManager"/> pipeline.
/// This interface standardizes the execution of flow steps by providing a common method that returns an <see cref="object"/>
/// representing the step's result (or <c>null</c> for void steps), allowing the <see cref="FlowConfiguration"/> and 
/// <see cref="FlowManager"/> to handle diverse step types uniformly. Implementations such as <see cref="VoidFlowStepWrapper"/>  and <see cref="TypedFlowStepWrapper{TResponse}"/> use this interface to integrate their specific step logic into the flow.
/// <br/><br/>
/// Example:
/// <code>
/// public class LogStep : IFlowStep
/// {
///     public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         Console.WriteLine("Logging...");
///         await Task.CompletedTask;
///     }
/// }
/// 
/// public class ComputeStep : IFlowStep<int>
/// {
///     public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
///     {
///         return 42;
///     }
/// }
/// 
/// IFlowStepWrapper voidWrapper = new VoidFlowStepWrapper(new LogStep());
/// IFlowStepWrapper typedWrapper = new TypedFlowStepWrapper<int>(new ComputeStep());
/// var context = new FlowContext();
/// var voidResult = await voidWrapper.ExecuteAsync(context, CancellationToken.None); // Returns null
/// var typedResult = await typedWrapper.ExecuteAsync(context, CancellationToken.None); // Returns 42
/// Console.WriteLine($"Void: {voidResult}, Typed: {typedResult}");
/// </code>
/// </summary>
public interface IFlowStepWrapper
{
    /// <summary>
    /// Executes the wrapped flow step asynchronously using the provided context and cancellation token.
    /// Returns the step's result as an <see cref="object"/> (for typed steps) or <c>null</c> (for void steps).
    /// This method enables the <see cref="FlowManager"/> to process all steps consistently, collecting results
    /// into a <see cref="FlowResult"/> for further processing or reporting.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var context = new FlowContext();
    /// IFlowStepWrapper wrapper = new TypedFlowStepWrapper<string>(new GreetingStep());
    /// var result = await wrapper.ExecuteAsync(context, CancellationToken.None);
    /// Console.WriteLine(result); // Outputs: "Hello, World!" (or null for void steps)
    /// </code>
    /// </summary>
    /// <param name="context">The execution context containing shared data for the flow step.</param>
    /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
    /// <returns>A task yielding the result of the step as an <see cref="object"/>, or <c>null</c> if the step does not produce a result.</returns>
    /// <exception cref="Exception">Thrown if the wrapped step encounters an error during execution.</exception>
    Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}