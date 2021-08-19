///****************************************************************************
// * Copyright (c) 2018.12 ~ 2020.5 liangxie
// * 
// * http://qframework.io
// * https://github.com/liangxiegame/QFramework
// * 
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files (the "Software"), to deal
// * in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:
// * 
// * The above copyright notice and this permission notice shall be included in
// * all copies or substantial portions of the Software.
// * 
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// * THE SOFTWARE.
// ****************************************************************************/

using System;
using DG.Tweening;

namespace QFramework
{
	public class DoTweenAction : ActionKitAction, IPoolable, IPoolType
	{

		private Func<Tweener> mTweenFactory;

		public static DoTweenAction Allocate(Func<Tweener> tweenFactory)
		{
			var action = SafeObjectPool<DoTweenAction>.Instance.Allocate();

			action.mTweenFactory = tweenFactory;

			return action;
		}

		private Tweener mTweener { get; set; }

		protected override void OnBegin()
		{
			mTweener = mTweenFactory.Invoke();

			mTweener.OnComplete(Finish);
		}

		protected override void OnDispose()
		{
			Recycle2Cache();
		}

		public void OnRecycled()
		{
			mTweenFactory = null;
			if (mTweener != null && mTweener.IsPlaying())
			{
				mTweener.Kill();
			}	
			mTweener = null;

		}

		public bool IsRecycled { get; set; }

		public void Recycle2Cache()
		{
			SafeObjectPool<DoTweenAction>.Instance.Recycle(this);
		}
	}
	
	public static partial class IActionChainExtention
	{
		public static IActionChain DOTween(this IActionChain selfChain, Func<Tweener> tweenFactory)
		{
			return selfChain.Append(DoTweenAction.Allocate(tweenFactory));
		}
	}
	
	
	public class DoTweenSequenceAction : ActionKitAction, IPoolable, IPoolType
	{

		private Func<DG.Tweening.Sequence> mTweenFactory;

		private DG.Tweening.Sequence mSequence { get; set; }
		public static DoTweenSequenceAction Allocate(Func<DG.Tweening.Sequence> tweenFactory)
		{
			var action = SafeObjectPool<DoTweenSequenceAction>.Instance.Allocate();

			action.mTweenFactory = tweenFactory;

			return action;
		}

		protected override void OnBegin()
		{
			mSequence = mTweenFactory.Invoke();

			mSequence.OnComplete(Finish);
		}

		protected override void OnDispose()
		{
			Recycle2Cache();
		}

		public void OnRecycled()
		{
			mTweenFactory = null;

			if (mSequence != null && mSequence.IsPlaying())
			{
				mSequence.Kill();
			}

			mSequence = null;
		}

		public bool IsRecycled { get; set; }

		public void Recycle2Cache()
		{
			SafeObjectPool<DoTweenSequenceAction>.Instance.Recycle(this);
		}
	}


	public static partial class IActionChainExtention
	{
		public static IActionChain DOTween(this IActionChain selfChain, Func<DG.Tweening.Sequence> sequenceFactory)
		{
			return selfChain.Append(DoTweenSequenceAction.Allocate(sequenceFactory));
		}
	}
}