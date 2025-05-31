using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	/// <summary>
	/// A Dictionary where multiple values can be associated to the same key
	/// </summary>
	/// <typeparam name="K">The key type</typeparam>
	/// <typeparam name="V">The value type</typeparam>
	internal class MultiDictionary<K, V>
	{
		Dictionary<K, List<V>> m_Map;
		V[] m_DefaultRet = new V[0];

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiDictionary{K, V}"/> class.
		/// </summary>
		public MultiDictionary()
		{
			m_Map = new Dictionary<K, List<V>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiDictionary{K, V}"/> class.
		/// </summary>
		/// <param name="eqComparer">The equality comparer to use in the underlying dictionary.</param>
		public MultiDictionary(IEqualityComparer<K> eqComparer)
		{
			m_Map = new Dictionary<K, List<V>>(eqComparer);
		}


		/// <summary>
		/// Adds the specified key. Returns true if this is the first value for a given key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool Add(K key, V value)
		{
			List<V> list;
			if (m_Map.TryGetValue(key, out list))
			{
				list.Add(value);
				return false;
			}
			else
			{
				list = new List<V>();
				list.Add(value);
				m_Map.Add(key, list);
				return true;
			}
		}

		/// <summary>
		/// Finds all the values associated with the specified key. 
		/// An empty collection is returned if not found.
		/// </summary>
		/// <param name="key">The key.</param>
		public IEnumerable<V> Find(K key)
		{
			List<V> list;
			if (m_Map.TryGetValue(key, out list))
				return list;
			else
				return m_DefaultRet;
		}

		/// <summary>
		/// Determines whether this contains the specified key 
		/// </summary>
		/// <param name="key">The key.</param>
		public bool ContainsKey(K key)
		{
			return m_Map.ContainsKey(key);
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		public IEnumerable<K> Keys
		{
			get { return m_Map.Keys; }
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			m_Map.Clear();
		}

		/// <summary>
		/// Removes the specified key and all its associated values from the multidictionary
		/// </summary>
		/// <param name="key">The key.</param>
		public void Remove(K key)
		{
			m_Map.Remove(key);
		}

		/// <summary>
		/// Removes the value. Returns true if the removed value was the last of a given key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool RemoveValue(K key, V value)
		{
			List<V> list;

			if (m_Map.TryGetValue(key, out list))
			{
				list.Remove(value);

				if (list.Count == 0)
				{
					Remove(key);
					return true;
				}
			}

			return false;
		}

	}
}
