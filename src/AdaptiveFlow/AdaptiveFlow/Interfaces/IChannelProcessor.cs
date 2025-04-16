namespace AdaptiveFlow;

/// <summary>
/// Defines an interface for processing flow contexts from a channel.
/// Provides a method for asynchronous processing.
/// </summary>
public interface IChannelProcessor
{
    /// <summary>
    /// Processes flow contexts asynchronously using the specified <see cref="FlowManager"/>.
    /// </summary>
    /// <param name="manager">The flow manager responsible for managing workflows and channels.</param>
    /// <returns>
    /// A task representing the asynchronous operation of processing flow contexts.
    /// </returns>
    Task ProcessAsync(FlowManager manager);
}