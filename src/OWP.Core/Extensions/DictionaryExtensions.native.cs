namespace OWP.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
    }

    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary[key];
    }
}