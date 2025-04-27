namespace AdaptiveFlow.Workbook;

/// <summary>
/// Manages a lightweight and efficient storage system for sharing data between steps.
/// </summary>
public sealed class FlowContext()
{
    private readonly ConcurrentDictionary<FlowKey, object> _contextValues = new();

    #region Set

    /// <summary>
    /// Attempts to add a value to the context, ensuring the key is unique.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The context key.</param>
    /// <param name="content">The value to store.</param>
    /// <returns>True if successfully added, false if the key already exists.</returns>
    public bool TrySet<T>(FlowKey key, T content)
    {
        if (content is null) throw new ArgumentNullException(nameof(content));

        return _contextValues.TryAdd(key, content);
    }

    /// <summary>
    /// Attempts to add a value using a string-based key.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The key as a string.</param>
    /// <param name="content">The value to store.</param>
    /// <returns>True if successfully added, false if the key already exists.</returns>
    public bool TrySet<T>(string key, T content) => TrySet(new FlowKey(key), content);

    #endregion Set

    #region Get

    /// <summary>
    /// Retrieves a value from the context, returning a default value if not found.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="defaultValue">The default value if the key is not found.</param>
    /// <returns>The stored value or the default value.</returns>
    public T GetOrDefault<T>(FlowKey key, T defaultValue = default!) => _contextValues.TryGetValue(key, out var content) && content is T typedValue ? typedValue : defaultValue;

    /// <summary>
    /// Asynchronously retrieves a value from the context.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="key">The key to retrieve.</param>
    /// <returns>A ValueTask containing the retrieved value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the key is not found or the type is incorrect.</exception>
    public ValueTask<T> GetAsync<T>(FlowKey key)
    {
        if (_contextValues.TryGetValue(key, out var content) && content is T typedValue)
            return ValueTask.FromResult(typedValue);

        throw new KeyNotFoundException($"Key '{key}' not found or incorrect type.");
    }

    #endregion Get

    #region Contains

    /// <summary>
    /// Checks whether a key exists in the context.
    /// </summary>
    /// <param name="key">The context key to check.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public bool Contains(FlowKey key) => _contextValues.ContainsKey(key);

    #endregion Contains

    #region Removes

    /// <summary>
    /// Removes the specified key from the context.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if successfully removed, false if the key does not exist.</returns>
    public bool Remove(FlowKey key) => _contextValues.TryRemove(key, out _);

    /// <summary>
    /// Clears all values from the context.
    /// </summary>
    public void Clear() => _contextValues.Clear();

    #endregion Removes
}