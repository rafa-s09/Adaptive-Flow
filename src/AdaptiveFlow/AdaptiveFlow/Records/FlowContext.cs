namespace AdaptiveFlow;

public class FlowContext
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object?>> _scopes = new();

    public FlowContext() => _scopes.TryAdd("default", new ConcurrentDictionary<string, object?>());

    public T? Get<T>(string key, string scope = "default")
    {
        if (_scopes.TryGetValue(scope, out var scopeData) && scopeData.TryGetValue(key, out var value))
            return (T?)value;
        return default;
    }

    public void Set<T>(string key, T value, string scope = "default")
    {
        var scopeData = _scopes.GetOrAdd(scope, _ => new ConcurrentDictionary<string, object?>());
        scopeData[key] = value;
    }

    public bool Remove(string key, string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) && scopeData.TryRemove(key, out _);
    }

    public bool Contains(string key, string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) && scopeData.ContainsKey(key);
    }

    public IReadOnlyDictionary<string, object?> AsReadOnly(string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) ? scopeData : new ConcurrentDictionary<string, object?>();
    }

    public FlowContext CreateScope(string scope)
    {
        _scopes.TryAdd(scope, new ConcurrentDictionary<string, object?>());
        return this;
    }

    public IReadOnlyDictionary<string, object?> GetLogData(string scope = "default")
    {
        return AsReadOnly(scope).Where(kv => kv.Key.StartsWith("log_")).ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public IReadOnlyDictionary<string, ConcurrentDictionary<string, object?>> GetAllScopes()
    {
        return _scopes;
    }
}

