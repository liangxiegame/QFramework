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
	using System.Linq;

	/// <summary>
	/// 序列执行节点
	/// </summary>
	public class SequenceNode : ExecuteNode
	{
		protected List<IExecuteNode> mNodeQueue = new List<IExecuteNode>();
		protected List<IExecuteNode> mExcutingQueue = new List<IExecuteNode>();
		
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
				mExcutingQueue.Add(node);
			}
			Completed = false;
		}

		protected override void OnExecute(float dt)
		{
			if (mExcutingQueue.Count > 0)
			{
				if (mExcutingQueue[0].Execute(dt))
				{
					mExcutingQueue.RemoveAt(0);
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
			foreach (var node in nodes)
			{
				mNodeQueue.Add(node);
				mExcutingQueue.Add(node);
			}
		}

		public SequenceNode Append(IExecuteNode appendedNode)
		{
			mNodeQueue.Add(appendedNode);
			mExcutingQueue.Add(appendedNode);
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