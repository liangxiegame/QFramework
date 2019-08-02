/****************************************************************************
 * Copyright (c) 2017 ~ 2018.8 liangxie
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

namespace QF.Action
{
	/// <inheritdoc />
	/// <summary>
	/// 延时执行节点
	/// </summary>
	public class DelayAction : NodeAction, IPoolable
	{
		public float DelayTime;

		public static DelayAction Allocate(float delayTime, System.Action onEndCallback = null)
		{
			var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
			retNode.DelayTime = delayTime;
			retNode.OnEndedCallback = onEndCallback;
			return retNode;
		}

		public DelayAction()
		{
		}

		public DelayAction(float delayTime)
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
			SafeObjectPool<DelayAction>.Instance.Recycle(this);
		}

		public void OnRecycled()
		{
            DelayTime = 0.0f;
			Reset();
		}

		public bool IsRecycled { get; set; }
		
//		[Test]
//		public static void Test()
//		{
//			var delayAction = Allocate(1.0f);
// 
//			while (!delayAction.Finished && delayAction.Execute(0.02f))
//			{
// 
//			}
// 
//			delayAction.Dispose();
// 
//			Assert.IsTrue(delayAction.IsRecycled);
//			Assert.AreEqual(delayAction.mCurrentSeconds, 0.0f);
//			Assert.AreEqual(delayAction.DelayTime, 0.0f);
//			Assert.AreEqual(delayAction.OnBeganCallback, null);
//			Assert.AreEqual(delayAction.OnEndedCallback, null);
//			Assert.AreEqual(delayAction.OnDisposedCallback, null);
//			Assert.IsFalse(delayAction.mDisposed);
//		}
	}
}