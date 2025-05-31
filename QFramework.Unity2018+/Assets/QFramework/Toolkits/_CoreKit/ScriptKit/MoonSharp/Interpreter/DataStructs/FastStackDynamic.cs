using System;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
#if USE_DYNAMIC_STACKS
	internal class FastStack<T> : FastStackDynamic<T>
	{
		public FastStack(int startingCapacity)
			: base(startingCapacity)
		{
		}
	}
#endif

	/// <summary>
	/// A non preallocated, non_fixed size stack
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class FastStackDynamic<T> : List<T>
	{
			public FastStackDynamic(int startingCapacity)
			: base(startingCapacity)
		{
		}


		public void Set(int idxofs, T item)
		{
			this[this.Count - 1 - idxofs] = item;
		}


		public T Push(T item)
		{
			this.Add(item);
			return item;
		}

		public void Expand(int size)
		{
			for(int i = 0; i < size; i++)
				this.Add(default(T));
		}

		public void Zero(int index)
		{
			this[index] = default(T);
		}

		public T Peek(int idxofs = 0)
		{
			T item = this[this.Count - 1 - idxofs];
			return item;
		}
		public void CropAtCount(int p)
		{
			RemoveLast(Count - p);
		}

		public void RemoveLast( int cnt = 1)
		{
			if (cnt == 1)
			{
				this.RemoveAt(this.Count - 1);
			}
			else
			{
				this.RemoveRange(this.Count - cnt, cnt);
			}
		}

		public T Pop()
		{
			T retval = this[this.Count - 1];
			this.RemoveAt(this.Count - 1);
			return retval;
		}
	}
}


