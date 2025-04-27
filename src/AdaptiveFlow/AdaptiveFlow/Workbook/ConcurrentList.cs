namespace AdaptiveFlow.Workbook;

/// <summary>
/// A thread-safe list that allows concurrent access and modification.
/// Provides advanced control features like removal, clearing, counting, and filtering.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class ConcurrentList<T>
{
    #region Private

    private readonly List<T> _list = [];
    private readonly object _lock = new();

    #endregion Private

    #region Functions

    /// <summary>
    /// Adds an item to the list in a thread-safe manner.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        lock (_lock)
            _list.Add(item);
    }

    /// <summary>
    /// Adds a collection of items to the list in a thread-safe manner.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    public void AddRange(IEnumerable<T> collection)
    {
        lock (_lock)
            _list.AddRange(collection);
    }

    /// <summary>
    /// Removes an item from the list in a thread-safe manner.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(T item)
    {
        lock (_lock)
            return _list.Remove(item);
    }

    /// <summary>
    /// Clears all items from the list in a thread-safe manner.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
            _list.Clear();
    }

    /// <summary>
    /// Gets the count of items in the list in a thread-safe manner.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
                return _list.Count;
        }
    }

    /// <summary>
    /// Counts the number of elements that satisfy a specified condition, in a thread-safe manner.
    /// </summary>
    /// <param name="predicate">The condition to test against elements.</param>
    /// <returns>The number of elements that satisfy the condition.</returns>
    public int CountWhere(Func<T, bool> predicate)
    {
        lock (_lock)
            return _list.Count(predicate);
    }

    /// <summary>
    /// Retrieves a snapshot of the list at the current moment in a thread-safe manner.
    /// </summary>
    /// <returns>An array containing all the elements in the list.</returns>
    public T[] Snapshot()
    {
        lock (_lock)
            return [.. _list];
    }

    /// <summary>
    /// Retrieves a filtered snapshot based on a condition in a thread-safe manner.
    /// </summary>
    /// <param name="predicate">The condition to filter items.</param>
    /// <returns>A list containing items that match the condition.</returns>
    public List<T> Filter(Func<T, bool> predicate)
    {
        lock (_lock)
            return [.. _list.Where(predicate)];
    }

    /// <summary>
    /// Checks if the list contains a specific item in a thread-safe manner.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>True if the item is found; otherwise, false.</returns>
    public bool Contains(T item)
    {
        lock (_lock)
            return _list.Contains(item);
    }

    /// <summary>
    /// Retrieves an enumerator for iterating over a snapshot of the list in a thread-safe manner.
    /// </summary>
    /// <returns>An enumerator for the list snapshot.</returns>
    public IEnumerator<T> ToEnumerator()
    {
        // Return a copy of the list's enumerator to ensure thread-safety
        lock (_lock)
            return _list.ToList().GetEnumerator();
    }

    /// <summary>
    /// Retrieves a thread-safe enumerable snapshot of the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the elements of the list.</returns>
    public IEnumerable<T> ToEnumerable()
    {
        return [.. _list];
    }

    /// <summary>
    /// Checks if any element matches the provided predicate.
    /// </summary>
    public bool Any(Func<T, bool> predicate)
    {
        // Safe evaluation inside the lock
        lock (_lock)
            return _list.Any(predicate);
    }

    /// <summary>
    /// Checks if the list contains any elements.
    /// </summary>
    public bool Any()
    {
        // Safe evaluation inside the lock
        lock (_lock)
            return _list.Any();
    }

    /// <summary>
    /// Executes an action for each item in the list in a thread-safe manner.
    /// </summary>
    /// <param name="action">The action to execute for each item.</param>
    public void ForEach(Action<T> action)
    {
        lock (_lock)
            foreach (var item in _list)
                action(item);
    }

    /// <summary>
    /// Retrieves the underlying list without thread safety. Use cautiously. <br/>
    /// <b>Warning: direct access, use only within controlled scenarios</b>
    /// </summary>
    public List<T> UnsafeList => _list;

    #endregion Functions
}