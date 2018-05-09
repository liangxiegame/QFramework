/****************************************************************************
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

namespace QFramework
{
	using System.Collections.Generic;

	/// <inheritdoc />
	/// <summary>
	/// 序列执行节点
	/// </summary>
	public class SequenceNode : NodeAction ,IPoolable
	{
		protected readonly List<IAction> mNodes = new List<IAction>();
		protected readonly List<IAction> mExcutingNodes = new List<IAction>();
		
		public bool Completed = false;

		public int TotalCount
		{
			get { return mExcutingNodes.Count; }
		}

		protected override void OnReset()
		{
			mExcutingNodes.Clear();
			foreach (var node in mNodes)
			{
				node.Reset();
				mExcutingNodes.Add(node);
			}
			Completed = false;
		}

		protected override void OnExecute(float dt)
		{
			if (mExcutingNodes.Count > 0)
			{
				// 如果有异常，则进行销毁，不再进行下边的操作
				if (mExcutingNodes[0].Disposed && !mExcutingNodes[0].Finished)
				{
					Dispose();
					return;
				}
 
				while (mExcutingNodes[0].Execute(dt))
				{
					mExcutingNodes.RemoveAt(0);
  
					if (mExcutingNodes.Count == 0)
					{
						break;
					}
				}
			}
 
			Finished = mExcutingNodes.Count == 0;
		}

		public static SequenceNode Allocate(params IAction[] nodes)
		{
			var retNode = SafeObjectPool<SequenceNode>.Instance.Allocate();
			foreach (var node in nodes)
			{
				retNode.mNodes.Add(node);
				retNode.mExcutingNodes.Add(node);
			}

			return retNode;
		}

		/// <summary>
		/// 不建议使用
		/// </summary>
		public SequenceNode(){}

		public SequenceNode Append(IAction appendedNode)
		{
			mNodes.Add(appendedNode);
			mExcutingNodes.Add(appendedNode);
			return this;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			
			SafeObjectPool<SequenceNode>.Instance.Recycle(this);
		}

		void IPoolable.OnRecycled()
		{
			mNodes.ForEach(node => node.Dispose());
			mNodes.Clear();

			mExcutingNodes.Clear();
		}

		bool IPoolable.IsRecycled { get; set; }
	}
}