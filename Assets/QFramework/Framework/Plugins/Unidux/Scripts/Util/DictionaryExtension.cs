using System.Collections.Generic;

namespace Unidux.Util
{
    public static class DictionaryExtension
    {
        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue
        )
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defaultValue;
        }

        public static TValue GetOrNull<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key
        )
            where TValue : class
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : null;
        }
    }
}