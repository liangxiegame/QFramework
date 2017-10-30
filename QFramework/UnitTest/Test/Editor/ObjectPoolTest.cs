/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

namespace QFramework.Test.Core
{
	using NUnit.Framework;

	public class ObjectPoolTest
	{
		//arrange
		class PoolObject : IPoolAble
		{
			public int id;

			public PoolObject()
			{
				id = 10;
			}

			public void OnRecycled()
			{
				id = 0;
			}

			public bool IsRecycled { get; set; }
		}

		[Test]
		public void ObjectPoolTest_AllocateAndRecycle_ObjectNull()
		{
			//option
			PoolObject po = SafeObjectPool<PoolObject>.Instance.Allocate();
			//Assert
			Assert.IsNotNull(po);
			Assert.AreEqual(10, po.id);

			//option
			bool isRecycle = SafeObjectPool<PoolObject>.Instance.Recycle(po);
			//Assert
			Assert.IsTrue(isRecycle);
			Assert.AreEqual(0, po.id);

			SafeObjectPool<PoolObject>.Instance.Dispose();
		}

		[Test]
		public void ObjectPoolTest_GetCurCountAndMaxCacheCount_Count()
		{
			//option
			SafeObjectPool<PoolObject>.Instance.Init(20, 5);
			SafeObjectPool<PoolObject>.Instance.MaxCacheCount = 20;
			//Asset
			Assert.AreEqual(20, SafeObjectPool<PoolObject>.Instance.MaxCacheCount);
			Assert.AreEqual(5, SafeObjectPool<PoolObject>.Instance.CurCount);
			//option
			SafeObjectPool<PoolObject>.Instance.Dispose();
		}

	}
}