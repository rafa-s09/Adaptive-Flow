namespace AdaptiveFlow;

public interface IFlowStep
{
    Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default);
}

