namespace AdaptiveFlow.Tests.Steps;

public class FailingStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new InvalidOperationException("Step failed intentionally");
    }
}
