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

namespace QFramework
{
	using System;

	/// <summary>
	/// 延时执行节点
	/// </summary>
	public class DelayNode : ExecuteNode, IPoolAble
	{
		public float DelayTime;

		public static DelayNode Allocate(float delayTime, Action onEndCallback = null)
		{
			var retNode = SafeObjectPool<DelayNode>.Instance.Allocate();
			retNode.DelayTime = delayTime;
			retNode.OnEndedCallback = onEndCallback;
			return retNode;
		}

		public DelayNode()
		{
		}

		public DelayNode(float delayTime)
		{
			DelayTime = delayTime;
		}

		private float mCurrentSeconds = 0.0f;

		protected override void OnReset()
		{
			mCurrentSeconds = 0.0f;
		}

		protected override void OnExecute(float dt)
		{
			mCurrentSeconds += dt;
			Finished = mCurrentSeconds >= DelayTime;
		}

		protected override void OnDispose()
		{
			SafeObjectPool<DelayNode>.Instance.Recycle(this);
		}

		public void OnRecycled()
		{
			Reset();
		}

		public bool IsRecycled { get; set; }
	}
}