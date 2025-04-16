namespace AdaptiveFlow;

public interface IChannelProcessor
{
    Task ProcessAsync(FlowManager manager);
}