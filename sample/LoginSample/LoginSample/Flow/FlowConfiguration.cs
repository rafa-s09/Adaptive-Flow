namespace LoginSample.Flow
{
    public class FlowConfiguration
    {
        public List<(IFlowStep Step, Func<FlowContext, bool> Condition)> Steps { get; } = new();
        public FlowConfiguration AddStep(IFlowStep step, Func<FlowContext, bool>? condition = null)
        {
            Steps.Add((step, condition ?? (_ => true)));
            return this;
        }
    }
}
