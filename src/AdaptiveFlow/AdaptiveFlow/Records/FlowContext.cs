namespace AdaptiveFlow;

/// <summary>
/// Represents a thread-safe context for storing and retrieving key-value pairs across different scopes.
/// Provides support for creating scopes, managing data, and retrieving log-related information.
/// </summary>
public class FlowContext
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object?>> _scopes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowContext"/> class and sets up the default scope.
    /// </summary>
    public FlowContext() => _scopes.TryAdd("default", new ConcurrentDictionary<string, object?>());

    // <summary>
    /// Retrieves a value of type <typeparamref name="T"/> associated with the specified key and scope.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="scope">The scope from which the value is to be retrieved. Defaults to "default".</param>
    /// <returns>The value if found, otherwise the default value of type <typeparamref name="T"/>.</returns>
    public T? Get<T>(string key, string scope = "default")
    {
        if (_scopes.TryGetValue(scope, out var scopeData) && scopeData.TryGetValue(key, out var value))
            return (T?)value;
        return default;
    }

    /// <summary>
    /// Sets a key-value pair in the specified scope.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to associate with the value.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="scope">The scope in which the key-value pair is set. Defaults to "default".</param>
    public void Set<T>(string key, T value, string scope = "default")
    {
        var scopeData = _scopes.GetOrAdd(scope, _ => new ConcurrentDictionary<string, object?>());
        scopeData[key] = value;
    }

    /// <summary>
    /// Removes a key-value pair from the specified scope.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <param name="scope">The scope from which to remove the key-value pair. Defaults to "default".</param>
    /// <returns>True if the key was successfully removed, otherwise false.</returns>
    public bool Remove(string key, string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) && scopeData.TryRemove(key, out _);
    }

    /// <summary>
    /// Checks if the specified key exists in the given scope.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="scope">The scope in which to check for the key. Defaults to "default".</param>
    /// <returns>True if the key exists, otherwise false.</returns>
    public bool Contains(string key, string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) && scopeData.ContainsKey(key);
    }

    /// <summary>
    /// Retrieves a read-only dictionary of key-value pairs from the specified scope.
    /// </summary>
    /// <param name="scope">The scope from which to retrieve data. Defaults to "default".</param>
    /// <returns>A read-only dictionary containing the data of the specified scope, or an empty dictionary if the scope does not exist.</returns>
    public IReadOnlyDictionary<string, object?> AsReadOnly(string scope = "default")
    {
        return _scopes.TryGetValue(scope, out var scopeData) ? scopeData : new ConcurrentDictionary<string, object?>();
    }

    /// <summary>
    /// Creates a new scope within the context.
    /// </summary>
    /// <param name="scope">The name of the new scope to create.</param>
    /// <returns>The current instance of <see cref="FlowContext"/> for method chaining.</returns>
    public FlowContext CreateScope(string scope)
    {
        _scopes.TryAdd(scope, new ConcurrentDictionary<string, object?>());
        return this;
    }

    /// <summary>
    /// Retrieves all log-related key-value pairs (keys starting with "log_") from the specified scope.
    /// </summary>
    /// <param name="scope">The scope from which to retrieve log-related data. Defaults to "default".</param>
    /// <returns>A read-only dictionary containing log-related key-value pairs from the specified scope.</returns>
    public IReadOnlyDictionary<string, object?> GetLogData(string scope = "default")
    {
        return AsReadOnly(scope).Where(kv => kv.Key.StartsWith("log_")).ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    /// <summary>
    /// Retrieves all scopes and their corresponding key-value pairs.
    /// </summary>
    /// <returns>A read-only dictionary where keys are scope names and values are dictionaries of key-value pairs within the scope.</returns>
    public IReadOnlyDictionary<string, ConcurrentDictionary<string, object?>> GetAllScopes()
    {
        return _scopes;
    }
}

