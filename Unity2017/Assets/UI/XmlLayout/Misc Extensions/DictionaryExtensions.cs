using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Xml
{
    public static class DictionaryExtensions
    {
        public static void AddIfKeyNotExists<TKey, TValue>(this IDictionary<TKey,TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
        }

        /*public static AttributeDictionary ToAttributeDictionary(this Dictionary<string, string> dictionary)
        {
            
        }*/
    }
}
