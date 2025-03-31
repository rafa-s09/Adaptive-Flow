namespace LoginSample.Flow
{
    public class FlowContext
    {
        public Dictionary<string, object> Data { get; } = new();
    }

    public class FlowContext<TModel>
    {
        public TModel? Data { get; set; }
    }
}
