namespace AdaptiveFlow;

public class DefaultChannelProcessor : IChannelProcessor
{
    public async Task ProcessAsync(FlowManager manager)
    {
        ChannelReader<FlowContext> channelReader = manager.GetChannel().Reader;
        while (await channelReader.WaitToReadAsync())
        {
            if (channelReader.TryRead(out var context))
            {
                FlowResult flowResult = await manager.ExecuteFlowAsync(context, CancellationToken.None);

                if (!flowResult.Success)
                {
                    var error = flowResult.Error;
                    manager.GetLogger()?.LogWarning( $"Flow execution failed. Code: {error?.Code}, Message: {error?.Message}, Step: {error?.StepName ?? "N/A"}");
                }
                else
                {
                    manager.GetLogger()?.LogInformation("Flow execution completed successfully.");
                }
            }
        }
    }
}