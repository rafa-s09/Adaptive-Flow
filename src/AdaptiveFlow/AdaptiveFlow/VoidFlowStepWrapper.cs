namespace AdaptiveFlow;

public class VoidFlowStepWrapper : IFlowStepWrapper
{
    private readonly IFlowStep _step;

    public VoidFlowStepWrapper(IFlowStep step)
    {
        _step = step ?? throw new ArgumentNullException(nameof(step), "The flow step cannot be null.");
    }

    public async Task<object?> ExecuteAsync(FlowContext context, CancellationToken cancellationToken)
    {
        await _step.ExecuteAsync(context, cancellationToken);
        return null;
    }
}