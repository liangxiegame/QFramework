/****************************************************************************
 * Copyright (c) 2018.12 liangxie
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

using System;

namespace QF.Action
{
	public class OnlyBeginAction : NodeAction,IPoolable,IPoolType
	{
		private Action<OnlyBeginAction> mBeginAction;

		public static OnlyBeginAction Allocate(Action<OnlyBeginAction> beginAction)
		{
			var retSimpleAction = SafeObjectPool<OnlyBeginAction>.Instance.Allocate();

			retSimpleAction.mBeginAction = beginAction;
			
			return retSimpleAction;
		}
		
		public void OnRecycled()
		{
			mBeginAction = null;
		}

		protected override void OnBegin()
		{
			if (mBeginAction != null)
			{
				mBeginAction.Invoke(this);
			}
		}

		public bool IsRecycled { get; set; }
		public void Recycle2Cache()
		{
			SafeObjectPool<OnlyBeginAction>.Instance.Recycle(this);
		}
	}
}