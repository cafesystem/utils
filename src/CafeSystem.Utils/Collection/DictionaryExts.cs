using System;
using System.Collections.Generic;

namespace CafeSystem.Utils;

public static class DictionaryExts
{
    /// <summary>
    /// Try to Add a value to the dictionary.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>Return true if Added, or false if failed.</returns>
    public static bool TryAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
    {
        try
        {
            dictionary.Add(key, value);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieve or add a value to dictionary
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <param name="key">key value</param>
    /// <param name="value">value</param>
    /// <returns>If the key already exists, return its value, if not try to add the key with its value and then return it.</returns>
    public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
    {
        dictionary.TryAdd(key, value);
        return dictionary[key];
    }

    /// <summary>
    /// Retrieve or add a value to dictionary via add function.
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <param name="key">key value</param>
    /// <param name="addFunc">add function returning the value</param>
    /// <returns>value</returns>
    public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> addFunc)
    {
        dictionary.TryAdd(key, addFunc());
        return dictionary[key];
    }

    /// <summary>
    /// Add source dictionary items to the target dictionary.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void AddRange<TK, TV>(this IDictionary<TK, TV> target, IDictionary<TK, TV> source)
    {
        foreach (var (key, value) in source)
        {
            target.AddOrUpdate(key, value, (_, _) => value);
        }
    }

    /// <summary>
    /// Add or update a key/value pair of the dictionary.  
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <param name="key">key value</param>
    /// <param name="value">value</param>
    /// <param name="updateFunc">update function</param>
    /// <returns>The value for the key.</returns>
    /// <remarks>
    /// Add or updates the key/value pair to the dictionary.
    /// </remarks>
    public static TV AddOrUpdate<TK, TV>(
        this IDictionary<TK, TV> dictionary, TK key, TV value, Func<TK, TV, TV> updateFunc)
    {
        return dictionary.AddOrUpdate(key, k => dictionary.GetOrAdd(k, value), updateFunc);
    }

    /// <summary>
    /// Add or update a key/value pair of the dictionary.  
    /// </summary>
    /// <typeparam name="TK">key type</typeparam>
    /// <typeparam name="TV">value type</typeparam>
    /// <param name="dictionary">dictionary</param>
    /// <param name="key">key value</param>
    /// <param name="addFunc">add function</param>
    /// <param name="updateFunc">update function</param>
    /// <returns></returns>
    public static TV AddOrUpdate<TK, TV>(
        this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> addFunc, Func<TK, TV, TV> updateFunc)
    {
        return AddOrUpdateInternal(dictionary, key, new Func<TK, TV>(addFunc), new Func<TK, TV, TV>(updateFunc));
    }


    /// <summary>
    /// Adds a key/value pair to the dictionary.  
    /// </summary>
    /// <typeparam name="TK">key type</typeparam>
    /// <typeparam name="TV">value type</typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="addFunc">add function</param>
    /// <param name="updateFunc">update function</param>
    /// <returns>The new value for the key.</returns>
    /// <remarks>
    /// Add or updates the key/value pair to the dictionary.
    /// </remarks>
    private static TV AddOrUpdateInternal<TK, TV>(
        IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> addFunc, Func<TK, TV, TV> updateFunc)
    {
        dictionary[key] = !dictionary.ContainsKey(key)
            ? addFunc.Invoke(key)
            : updateFunc.Invoke(key, dictionary[key]);

        return dictionary[key];
    }
}