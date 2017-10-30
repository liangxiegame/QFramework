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
	using System.Linq;
	using System.Collections.Generic;
	
	/// <summary>
	/// 并发执行的协程
	/// </summary>
	public class SpawnNode :ExecuteNode 
	{
		protected List<IExecuteNode> mNodeList = new List<IExecuteNode>();

		protected override void OnReset()
		{
			foreach (var executeNode in mNodeList)
			{
				executeNode.Reset();
			}
		}
		
		protected override void OnExecute(float dt)
		{
			foreach (var node in mNodeList.Where(node => !node.Finished))
			{
				if (node.Execute(dt))
				{
					Finished = mNodeList.Where(n => !n.Finished).Count() == 0;
				}
			}
		}
		
		public SpawnNode(params IExecuteNode[] nodes)
		{
			mNodeList.AddRange (nodes);
		}

//		[Obsolete("Deprecated since 0.0.5")]
//		public SpawnNode(object deprecated,params IExecuteNode[] nodes)
//		{
//			mNodeList.AddRange (nodes);
//		}
		
		protected override void OnDispose()
		{
			foreach (var node in mNodeList)
			{
				node.Dispose();
			}
			mNodeList.Clear();
			mNodeList = null;
		}
	}
}