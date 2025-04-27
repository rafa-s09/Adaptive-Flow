namespace AdaptiveFlow.Workbook;

/// <summary>
/// Represents a unique key for identifying values.
/// </summary>
/// <param name="name">The key name.</param>
/// <exception cref="ArgumentNullException">Thrown if the name is null.</exception>
public readonly struct FlowKey(string name)
{
    private readonly string _name = name ?? throw new ArgumentNullException(nameof(name));
    private readonly int _hash = ComputeHash(name);

    #region Get

    /// <summary>
    /// The name of the context key.
    /// </summary>
    public string Name => _name; // Exposes name safely

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The computed hash code.</returns>
    public override int GetHashCode() => _hash;

    #endregion Get

    #region MurmurHash3 algorithm

    /// <summary>
    /// Generates an efficient hash based on the MurmurHash3 algorithm.
    /// </summary>
    /// <param name="name">The name to be hashed.</param>
    /// <returns>The generated hash value.</returns>
    private static int ComputeHash(ReadOnlySpan<char> name)
    {
        const uint seed = 17; 
        uint hash = seed;

        foreach (char c in name)
        {
            uint k = (uint)c;
            k *= 0xcc9e2d51;
            k = (k << 15) | (k >> 17); 
            k *= 0x1b873593;

            hash ^= k;
            hash = (hash << 13) | (hash >> 19); 
            hash = hash * 5 + 0xe6546b64;
        }

        hash ^= (uint)name.Length;
        hash ^= (hash >> 16);
        hash *= 0x85ebca6b;
        hash ^= (hash >> 13);
        hash *= 0xc2b2ae35;
        hash ^= (hash >> 16);

        return (int)hash;
    }

    #endregion MurmurHash3 algorithm

    #region Operators

    /// <summary>
    /// Checks if two ContextKey instances are equal.
    /// </summary>
    /// <param name="other">The ContextKey to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public bool Equals(FlowKey other) => _name == other._name && _hash == other._hash;

    /// <summary>
    /// Checks if two ContextKey instances are equal.
    /// </summary>
    /// <param name="obj">The ContextKey to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public override bool Equals(object? obj) => obj is FlowKey other && Equals(other);

    /// <summary>
    /// Determines whether two ContextKey instances are equal.
    /// </summary>
    /// <param name="left">The first ContextKey to compare.</param>
    /// <param name="right">The second ContextKey to compare.</param>
    /// <returns>True if both instances are equal, otherwise false.</returns>
    public static bool operator ==(FlowKey left, FlowKey right) => left.Equals(right);

    /// <summary>
    /// Determines whether two ContextKey instances are not equal.
    /// </summary>
    /// <param name="left">The first ContextKey to compare.</param>
    /// <param name="right">The second ContextKey to compare.</param>
    /// <returns>True if the instances are different, otherwise false.</returns>
    public static bool operator !=(FlowKey left, FlowKey right) => !left.Equals(right);

    /// <summary>
    /// Returns the string representation of this context key.
    /// </summary>
    /// <returns>The name of the key.</returns>
    public override string ToString() => Name;

    #endregion Operators
}