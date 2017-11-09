/****************************************************************************
 * Copyright (c) 2017 liangxie
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

using UnityEngine;

namespace QFramework.Example
{
	public class SafeObjectPoolExample : MonoBehaviour
	{
		class Msg : IPoolAble,IPoolType
		{
			#region IPoolAble 实现

			public void OnRecycled()
			{
				Log.I("OnRecycled");
			}
			
			public bool IsRecycled { get; set; }

			#endregion
		
			
			#region IPoolType 实现

			public static Msg Allocate()
			{
				return SafeObjectPool<Msg>.Instance.Allocate();
			}
			
			public void Recycle2Cache()
			{
				SafeObjectPool<Msg>.Instance.Recycle(this);
			}
			
			#endregion
		}
		
		private void Start()
		{
			SafeObjectPool<Msg>.Instance.Init(100, 50);			
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			var fishOne = Msg.Allocate();
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			fishOne.Recycle2Cache();

			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
			
			for (int i = 0; i < 10; i++)
			{
				Msg.Allocate();
			}
			
			Log.I("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount);
		}
	}
}