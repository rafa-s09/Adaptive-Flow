namespace AdaptiveFlow.Tests.Steps;

public class DelayedStep : IFlowStep
{
    private readonly int _delayMs;
    public DelayedStep(int delayMs = 500) => _delayMs = delayMs;

    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(_delayMs, cancellationToken);
        context.Data.TryAdd("Delayed", true);
    }
}