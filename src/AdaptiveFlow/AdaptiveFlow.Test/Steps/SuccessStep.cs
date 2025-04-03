namespace AdaptiveFlow.Tests.Steps;

public class SuccessStep : IFlowStep
{
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        context.Data.TryAdd("Success", true);
        await Task.CompletedTask;
    }
}
