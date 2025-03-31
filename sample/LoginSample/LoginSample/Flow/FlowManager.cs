namespace LoginSample.Flow
{
    public class FlowManager
    {
        private readonly FlowConfiguration _config;

        public FlowManager(FlowConfiguration config)
        {
            _config = config;
        }

        public async Task RunAsync(FlowContext context)
        {
            foreach (var (step, condition) in _config.Steps)
            {
                if (!condition(context)) continue;
                await step.ExecuteAsync(context);
            }
        }
    }
}
