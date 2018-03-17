/****************************************************************************
 * Copyright (c) 2017 liangxie
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
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
	using System;
	using UnityEngine;

	/// <summary>
	/// Node类将处理代表我们地图的2D格子中其中的每个格子对象,一下是Node.cs文件.
	/// </summary>
	public class AStarNode : IComparable
	{
		public float NodeTotalCost; // G 它是从开始节点到当前节点的代价值
		public float EstimatedCost; // H 它是从当前节点到目标节点的估计值
		public bool IsObstacle;
		public AStarNode Parent;
		public Vector3 Position;

		public AStarNode()
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
		}

		public AStarNode(Vector3 pos)
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
			Position = pos;
		}

		public void MarkAsObstacle()
		{
			IsObstacle = true;
		}

		public int CompareTo(object obj)
		{
			AStarNode node = obj as AStarNode;
			return EstimatedCost.CompareTo(node.EstimatedCost);
		}
	}
}