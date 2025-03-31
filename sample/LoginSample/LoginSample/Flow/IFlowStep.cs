namespace LoginSample.Flow
{
    public interface IFlowStep
    {
        Task ExecuteAsync(FlowContext context);
    }
}
