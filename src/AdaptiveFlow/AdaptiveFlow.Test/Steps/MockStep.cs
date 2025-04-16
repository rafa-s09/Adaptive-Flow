namespace AdaptiveFlow.Tests.Steps;

/// <summary>
/// Um mock para testes de passos
/// </summary>
public class MockStep : IFlowStep
{
    public Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}