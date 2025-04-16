namespace AdaptiveFlow;

public interface IFlowStepWrapper
{
    Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}