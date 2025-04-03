namespace AdaptiveFlow.Tests.Steps;

public class ComputeStep : IFlowStep<int>
{
    public async Task<int> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return 42;
    }
}