/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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

	/// <summary>
	/// 序列执行节点
	/// </summary>
	public class SequenceNode : ExecuteNode ,IPoolable
	{
		protected readonly List<IExecuteNode> mNodes = new List<IExecuteNode>();
		protected readonly List<IExecuteNode> mExcutingNodes = new List<IExecuteNode>();
		
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
				if (mExcutingNodes[0].Execute(dt))
				{
					mExcutingNodes.RemoveAt(0);
				}
			} 
			else
			{
				Finished = true;
				Completed = true;
			}
		}

		public static SequenceNode Allocate(params IExecuteNode[] nodes)
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

		public SequenceNode Append(IExecuteNode appendedNode)
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