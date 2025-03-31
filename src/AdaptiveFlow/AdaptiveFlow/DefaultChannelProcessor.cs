namespace AdaptiveFlow;

/// <summary>
/// Provides a default implementation of <see cref="IChannelProcessor"/> for processing flow contexts in the <see cref="FlowManager"/>.
/// Reads contexts from the channel and executes them sequentially, logging results if a logger is available.
/// </summary>
public class DefaultChannelProcessor : IChannelProcessor
{
    /// <summary>
    /// Processes flow contexts from the channel of the specified <see cref="FlowManager"/>.
    /// Continues reading and executing contexts until the channel is closed or an unhandled exception occurs.
    /// </summary>
    /// <param name="manager">The <see cref="FlowManager"/> instance to process contexts for.</param>
    /// <returns>A task representing the asynchronous processing of the channel.</returns>
    public async Task ProcessAsync(FlowManager manager)
    {
        ChannelReader<FlowContext> channelReader = manager.GetChannelReader();
        while (await channelReader.WaitToReadAsync())
        {
            if (channelReader.TryRead(out var context))
            {
                FlowResult flowResult = await manager.ExecuteFlowAsync(context, CancellationToken.None);
                if (!flowResult.Success)                
                    manager.GetLogger()?.LogWarning("Flow execution completed with failure: {Error}", flowResult.ErrorMessage);                
                else                
                    manager.GetLogger()?.LogInformation("Flow execution completed successfully.");                
            }
        }
    }
}