// ----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exit Games GmbH">
//   Photon Extensions - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Provides some helpful methods and extensions for Hashtables, etc.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System.Collections;
	using System.Collections.Generic;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    #endif
    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>
    /// This static class defines some useful extension methods for several existing classes (e.g. Vector3, float and others).
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Merges all keys from addHash into the target. Adds new keys and updates the values of existing keys in target.
        /// </summary>
        /// <param name="target">The IDictionary to update.</param>
        /// <param name="addHash">The IDictionary containing data to merge into target.</param>
        public static void Merge(this IDictionary target, IDictionary addHash)
        {
            if (addHash == null || target.Equals(addHash))
            {
                return;
            }

            foreach (object key in addHash.Keys)
            {
                target[key] = addHash[key];
            }
        }

        /// <summary>
        /// Merges keys of type string to target Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not remove keys from target (so non-string keys CAN be in target if they were before).
        /// </remarks>
        /// <param name="target">The target IDictionary passed in plus all string-typed keys from the addHash.</param>
        /// <param name="addHash">A IDictionary that should be merged partly into target to update it.</param>
        public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
        {
            if (addHash == null || target.Equals(addHash))
            {
                return;
            }

            foreach (object key in addHash.Keys)
            {
                // only merge keys of type string
                if (key is string)
                {
                    target[key] = addHash[key];
                }
            }
        }

        /// <summary>Helper method for debugging of IDictionary content, including type-information. Using this is not performant.</summary>
        /// <remarks>Should only be used for debugging as necessary.</remarks>
        /// <param name="origin">Some Dictionary or Hashtable.</param>
        /// <returns>String of the content of the IDictionary.</returns>
        public static string ToStringFull(this IDictionary origin)
        {
            return SupportClass.DictionaryToString(origin, false);
        }

		/// <summary>Helper method for debugging of List<T> content. Using this is not performant.</summary>
		/// <remarks>Should only be used for debugging as necessary.</remarks>
		/// <param name="data">Any List<T> where T implements .ToString().</param>
		/// <returns>A comma-separated string containing each value's ToString().</returns>
		public static string ToStringFull<T>(this List<T> data)
		{
			if (data == null) return "null";

			string[] sb = new string[data.Count];
			for (int i = 0; i < data.Count; i++)
			{
				object o = data[i];
				sb[i] = (o != null) ? o.ToString() : "null";
			}

			return string.Join(", ", sb);
		}

        /// <summary>Helper method for debugging of object[] content. Using this is not performant.</summary>
        /// <remarks>Should only be used for debugging as necessary.</remarks>
        /// <param name="data">Any object[].</param>
        /// <returns>A comma-separated string containing each value's ToString().</returns>
        public static string ToStringFull(this object[] data)
        {
            if (data == null) return "null";

            string[] sb = new string[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                object o = data[i];
                sb[i] = (o != null) ? o.ToString() : "null";
            }

            return string.Join(", ", sb);
        }


        /// <summary>
        /// This method copies all string-typed keys of the original into a new Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not recurse (!) into hashes that might be values in the root-hash.
        /// This does not modify the original.
        /// </remarks>
        /// <param name="original">The original IDictonary to get string-typed keys from.</param>
        /// <returns>New Hashtable containing only string-typed keys of the original.</returns>
        public static Hashtable StripToStringKeys(this IDictionary original)
        {
            Hashtable target = new Hashtable();
            if (original != null)
            {
                foreach (object key in original.Keys)
                {
                    if (key is string)
                    {
                        target[key] = original[key];
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// This method copies all string-typed keys of the original into a new Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not recurse (!) into hashes that might be values in the root-hash.
        /// This does not modify the original.
        /// </remarks>
        /// <param name="original">The original IDictonary to get string-typed keys from.</param>
        /// <returns>New Hashtable containing only string-typed keys of the original.</returns>
        public static Hashtable StripToStringKeys(this Hashtable original)
        {
            Hashtable target = new Hashtable();
            if (original != null)
            {
                foreach (DictionaryEntry entry in original)
                {
                    if (entry.Key is string)
                    {
                        target[entry.Key] = original[entry.Key];
                    }
                }
            }

            return target;
        }


        /// <summary>Used by StripKeysWithNullValues.</summary>
        /// <remarks>
        /// By making keysWithNullValue a static variable to clear before using, allocations only happen during the warm-up phase
        /// as the list needs to grow. Once it hit the high water mark for keys you need to remove.
        /// </remarks>
        private static readonly List<object> keysWithNullValue = new List<object>();

        /// <summary>Removes all keys with null values.</summary>
        /// <remarks>
        /// Photon properties are removed by setting their value to null. Changes the original IDictionary!
        /// Uses lock(keysWithNullValue), which should be no problem in expected use cases.
        /// </remarks>
        /// <param name="original">The IDictionary to strip of keys with null value.</param>
        public static void StripKeysWithNullValues(this IDictionary original)
        {
            lock (keysWithNullValue)
            {
                keysWithNullValue.Clear();

                foreach (DictionaryEntry entry in original)
                {
                    if (entry.Value == null)
                    {
                        keysWithNullValue.Add(entry.Key);
                    }
                }

                for (int i = 0; i < keysWithNullValue.Count; i++)
                {
                    var key = keysWithNullValue[i];
                    original.Remove(key);
                }
            }
        }

        /// <summary>Removes all keys with null values.</summary>
        /// <remarks>
        /// Photon properties are removed by setting their value to null. Changes the original IDictionary!
        /// Uses lock(keysWithNullValue), which should be no problem in expected use cases.
        /// </remarks>
        /// <param name="original">The IDictionary to strip of keys with null value.</param>
        public static void StripKeysWithNullValues(this Hashtable original)
        {
            lock (keysWithNullValue)
            {
                keysWithNullValue.Clear();

                foreach (DictionaryEntry entry in original)
                {
                    if (entry.Value == null)
                    {
                        keysWithNullValue.Add(entry.Key);
                    }
                }

                for (int i = 0; i < keysWithNullValue.Count; i++)
                {
                    var key = keysWithNullValue[i];
                    original.Remove(key);
                }
            }
        }


        /// <summary>
        /// Checks if a particular integer value is in an int-array.
        /// </summary>
        /// <remarks>This might be useful to look up if a particular actorNumber is in the list of players of a room.</remarks>
        /// <param name="target">The array of ints to check.</param>
        /// <param name="nr">The number to lookup in target.</param>
        /// <returns>True if nr was found in target.</returns>
        public static bool Contains(this int[] target, int nr)
        {
            if (target == null)
            {
                return false;
            }

            for (int index = 0; index < target.Length; index++)
            {
                if (target[index] == nr)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

