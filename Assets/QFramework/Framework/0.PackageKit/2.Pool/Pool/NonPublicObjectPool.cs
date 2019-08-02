/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QF
{
    using System;

	/// <summary>
	/// Object pool 4 class who no public constructor
	/// such as SingletonClass.QEventSystem
	/// </summary>
	public class NonPublicObjectPool<T> :Pool<T>,ISingleton where T : class,IPoolable
	{
		#region Singleton
		public void OnSingletonInit(){}
		
		public static NonPublicObjectPool<T> Instance
		{
			get { return SingletonProperty<NonPublicObjectPool<T>>.Instance; }
		}

		protected NonPublicObjectPool()
		{
			mFactory = new NonPublicObjectFactory<T>();
		}
		
		public void Dispose()
		{
			SingletonProperty<NonPublicObjectPool<T>>.Dispose();
		}
		#endregion

		/// <summary>
		/// Init the specified maxCount and initCount.
		/// </summary>
		/// <param name="maxCount">Max Cache count.</param>
		/// <param name="initCount">Init Cache count.</param>
		public void Init(int maxCount, int initCount)
		{
			if (maxCount > 0)
			{
				initCount = Math.Min(maxCount, initCount);
			}

			if (CurCount >= initCount) return;
			
			for (var i = CurCount; i < initCount; ++i)
			{
				Recycle(mFactory.Create());
			}
		}

		/// <summary>
		/// Gets or sets the max cache count.
		/// </summary>
		/// <value>The max cache count.</value>
		public int MaxCacheCount
		{
			get { return mMaxCount; }
			set
			{
				mMaxCount = value;

				if (mCacheStack == null) return;
				if (mMaxCount <= 0) return;
				if (mMaxCount >= mCacheStack.Count) return;
				var removeCount = mMaxCount - mCacheStack.Count;
				while (removeCount > 0)
				{
					mCacheStack.Pop();
					--removeCount;
				}
			}
		}

		/// <summary>
		/// Allocate T instance.
		/// </summary>
		public override T Allocate()
		{
			var result = base.Allocate();
			result.IsRecycled = false;
			return result;
		}

		/// <summary>
		/// Recycle the T instance
		/// </summary>
		/// <param name="t">T.</param>
		public override bool Recycle(T t)
		{
			if (t == null || t.IsRecycled)
			{
				return false;
			}

			if (mMaxCount > 0)
			{
				if (mCacheStack.Count >= mMaxCount)
				{
					t.OnRecycled();
					return false;
				}
			}

			t.IsRecycled = true;
			t.OnRecycled();
			mCacheStack.Push(t);

			return true;
		}
	}
}