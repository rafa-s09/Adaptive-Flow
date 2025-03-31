namespace AdaptiveFlow;

/// <summary>
/// Defines a processor for handling flow contexts from a channel asynchronously.
/// Implementations are responsible for reading and executing queued contexts in the <see cref="FlowManager"/>.
/// This abstraction enhances testability by allowing the channel processing logic to be mocked or customized.
/// <br/><br/>
/// Example:
/// <code>
/// public class CustomProcessor : IChannelProcessor
/// {
///     public async Task ProcessAsync(FlowManager manager)
///     {
///         while (await manager._channel.Reader.WaitToReadAsync())
///         {
///             if (manager._channel.Reader.TryRead(out var context))
///             {
///                 await manager.ExecuteFlowAsync(context, CancellationToken.None);
///             }
///         }
///     }
/// }
/// </code>
/// </summary>
public interface IChannelProcessor
{
    /// <summary>
    /// Processes flow contexts from the channel managed by the <see cref="FlowManager"/>.
    /// </summary>
    /// <param name="manager">The <see cref="FlowManager"/> instance providing access to the channel and execution logic.</param>
    /// <returns>A task representing the asynchronous processing of the channel.</returns>
    Task ProcessAsync(FlowManager manager);
}