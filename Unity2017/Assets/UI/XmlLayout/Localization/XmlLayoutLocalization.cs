using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Xml
{
    [CreateAssetMenu(fileName = "New Localization File", menuName = "XmlLayout/Localization/New Localization File")]
    public class XmlLayoutLocalization : ScriptableObject
    {
        [SerializeField]
        public LocalizationDictionary strings = new LocalizationDictionary();

        /// <summary>
        /// Get a specific string from this localization file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return strings.ContainsKey(key) ? strings[key] : "";
        }

        /// <summary>
        /// Set a specific string for this localization file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetString(string key, string value)
        {
            if (!strings.ContainsKey(key))
            {
                strings.Add(key, value);
            }
            else
            {
                strings[key] = value;
            }
        }

        /// <summary>
        /// Set all of the strings for this Localization data
        /// </summary>
        /// <param name="newStrings"></param>
        /// <param name="clearExisting">If this is set to true, then any existing strings will be cleared first.</param>
        public void SetStrings(IDictionary<string, string> newStrings, bool clearExisting = true)
        {
            if (clearExisting) strings.Clear();

            foreach (var kvp in newStrings)
            {
                SetString(kvp.Key, kvp.Value);
            }
        }

        [Serializable]
        public class LocalizationDictionary : SerializableDictionary<string, string>
        {            
            public LocalizationDictionary()
            {
                _Comparer = StringComparer.OrdinalIgnoreCase;
            }
        }        
    }
}
