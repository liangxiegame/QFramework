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
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// 序列执行节点
	/// </summary>
	public class SequenceNode : ExecuteNode
	{
		protected Queue<IExecuteNode> mNodeQueue = new Queue<IExecuteNode>();
		protected Queue<IExecuteNode> mExcutingQueue = new Queue<IExecuteNode>();
		
		public bool Completed = false;

		public int TotalCount
		{
			get { return mExcutingQueue.Count; }
		}

		protected override void OnReset()
		{
			mExcutingQueue.Clear();
			foreach (var node in mNodeQueue)
			{
				node.Reset();
				mExcutingQueue.Enqueue(node);
			}
			Completed = false;
		}

		protected override void OnExecute(float dt)
		{
			if (mExcutingQueue.Count > 0)
			{
				if (mExcutingQueue.First().Execute(dt))
				{
					mExcutingQueue.Dequeue();
				}
			} 
			else
			{
				Finished = true;
				Completed = true;
			}
		}

		public SequenceNode(params IExecuteNode[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++) 
			{
				mNodeQueue.Enqueue (nodes[i]);
				mExcutingQueue.Enqueue(nodes[i]);
			}
		}

		public SequenceNode Append(IExecuteNode appendedNode)
		{
			mNodeQueue.Enqueue(appendedNode);
			mExcutingQueue.Enqueue(appendedNode);
			return this;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			
			if (null != mNodeQueue)
			{
				mNodeQueue.ForEach(node => node.Dispose());
				mNodeQueue.Clear();
				mNodeQueue = null;
			}

			if (null != mExcutingQueue)
			{
				mExcutingQueue.Clear();
				mExcutingQueue = null;
			}
		}	
	}
}