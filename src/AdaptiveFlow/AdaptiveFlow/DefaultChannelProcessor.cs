namespace AdaptiveFlow;

/// <summary>
/// The default implementation of the <see cref="IChannelProcessor"/> interface.
/// Processes flow contexts from a channel and executes them using a <see cref="FlowManager"/>.
/// </summary>
public class DefaultChannelProcessor : IChannelProcessor
{
    /// <summary>
    /// Reads flow contexts from the channel managed by the <see cref="FlowManager"/>, 
    /// executes them asynchronously, and logs the results.
    /// </summary>
    /// <param name="manager">The <see cref="FlowManager"/> responsible for managing the workflow execution and channel.</param>
    /// <returns>A task that represents the asynchronous operation of processing flow contexts.</returns>
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