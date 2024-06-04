namespace Modules.Common.OBSOLETE.Mediator;

public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
{
    /// <summary>
    /// Checks if the key is already present
    /// </summary>
    /// <param name="key"></param>
    private void EnsureKey(TKey key)
    {
        if (!ContainsKey(key))
        {
            this[key] = new List<TValue>(1);
        }
        else if (this[key] == null)
        {
            this[key] = new List<TValue>(1);
        }
    }

    /// <summary>
    /// Adds a new value in the Values collection
    /// </summary>
    /// <param name="key">The key where to place the item in the value list</param>
    /// <param name="newItem">The new item to add</param>
    public void AddValue(TKey key, TValue newItem)
    {
        EnsureKey(key);
        this[key].Add(newItem);
    }

    /// <summary>
    /// Adds a list of values to append to the value collection
    /// </summary>
    /// <param name="key">The key where to place the item in the value list</param>
    /// <param name="newItems">The new items to add</param>
    public void AddValues(TKey key, IEnumerable<TValue> newItems)
    {
        EnsureKey(key);
        this[key].AddRange(newItems);
    }

    /// <summary>
    /// Removes a specific element from the dictionary.
    /// If the value list is empty the key is removed from the dictionary.
    /// </summary>
    /// <param name="key">The key from where to remove the value</param>
    /// <param name="value">The value to remove</param>
    /// <returns>Returns false if the key was not found</returns>
    public bool RemoveValue(TKey key, TValue value)
    {
        if (!ContainsKey(key))
        {
            return false;
        }

        this[key].Remove(value);

        if (this[key].Count == 0)
        {
            Remove(key);
        }

        return true;
    }

    /// <summary>
    /// Removes all items that match the predicate.
    /// If the value list is empty the key is removed from the dictionary
    /// </summary>
    /// <param name="key">The key from where to remove the value</param>
    /// <param name="match">Returns false if the key was not found</param>
    /// <returns>Returns false if the key was not found</returns>
    public bool RemoveAllValue(TKey key, Predicate<TValue> match)
    {
        if (!ContainsKey(key))
        {
            return false;
        }

        this[key].RemoveAll(match);

        if (this[key].Count == 0)
        {
            Remove(key);
        }

        return true;
    }
}