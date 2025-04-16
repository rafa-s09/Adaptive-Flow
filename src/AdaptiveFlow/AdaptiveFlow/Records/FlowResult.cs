namespace AdaptiveFlow;

public record FlowResult(bool Success, FlowError? Error = null, IReadOnlyDictionary<string, object?>? Results = null)
{
    public static FlowResult Ok(IReadOnlyDictionary<string, object?> results) => new(true, null, results);
    public static FlowResult Ok() => new(true, null, new Dictionary<string, object?>());
    public static FlowResult Fail(FlowError error) => new(false, error);
}
