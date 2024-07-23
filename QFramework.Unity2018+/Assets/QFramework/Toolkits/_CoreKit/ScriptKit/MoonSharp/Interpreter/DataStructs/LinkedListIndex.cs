using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	/// <summary>
	/// An index to accelerate operations on a LinkedList<typeparamref name="TValue"/> using a single key of type <typeparamref name="TKey"/>
	/// More than one LinkedListIndex can index the same linked list, but every node in the linked list must be indexed by one and only one
	/// LinkedListIndex object.
	/// </summary>
	/// <typeparam name="TKey">The type of the key. Must implement Equals and GetHashCode appropriately.</typeparam>
	/// <typeparam name="TValue">The type of the values contained in the linked list.</typeparam>
	internal class LinkedListIndex<TKey, TValue>
	{
		LinkedList<TValue> m_LinkedList;
		Dictionary<TKey, LinkedListNode<TValue>> m_Map = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedListIndex{TKey, TValue}"/> class.
		/// </summary>
		/// <param name="linkedList">The linked list to be indexed.</param>
		public LinkedListIndex(LinkedList<TValue> linkedList)
		{
			m_LinkedList = linkedList;
		}

		/// <summary>
		/// Finds the node indexed by the specified key, or null.
		/// </summary>
		/// <param name="key">The key.</param>
		public LinkedListNode<TValue> Find(TKey key)
		{
			LinkedListNode<TValue> node;

			if (m_Map == null)
				return null;

			if (m_Map.TryGetValue(key, out node))
				return node;

			return null;
		}

		/// <summary>
		/// Updates or creates a new node in the linked list, indexed by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>The previous value of the element</returns>
		public TValue Set(TKey key, TValue value)
		{
			LinkedListNode<TValue> node = Find(key);

			if (node == null)
			{
				Add(key, value);
				return default(TValue);
			}
			else
			{
				TValue val = node.Value;
				node.Value = value;
				return val;
			}
		}

		/// <summary>
		/// Creates a new node in the linked list, indexed by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(TKey key, TValue value)
		{
			var node = m_LinkedList.AddLast(value);

			if (m_Map == null)
				m_Map = new Dictionary<TKey, LinkedListNode<TValue>>();

			m_Map.Add(key, node);
		}

		/// <summary>
		/// Removes the specified key from the index, and the node indexed by the key from the linked list.
		/// </summary>
		/// <param name="key">The key.</param>
		public bool Remove(TKey key)
		{
			LinkedListNode<TValue> node = Find(key);

			if (node != null)
			{
				m_LinkedList.Remove(node);
				return m_Map.Remove(key);
			}

			return false;
		}


		/// <summary>
		/// Determines whether the index contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public bool ContainsKey(TKey key)
		{
			if (m_Map == null)
				return false;

			return m_Map.ContainsKey(key);
		}

		/// <summary>
		/// Clears this instance (removes all elements)
		/// </summary>
		public void Clear()
		{
            if(m_Map != null)
			    m_Map.Clear();
		}
	}
}
