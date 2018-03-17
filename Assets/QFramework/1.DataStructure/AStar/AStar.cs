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
    using UnityEngine;
    using System.Collections.Generic;

    public class AStar
    {
        public static PriorityQueue ClosedList, OpenList;

        private static float HeuristicEstimateCost(AStarNode curNode, AStarNode goalNode)
        {
            Vector3 vecCost = curNode.Position - goalNode.Position;
            return vecCost.magnitude;
        }

        public static List<AStarNode> FindPath(AStarNode start, AStarNode goal)
        {
            OpenList = new PriorityQueue();
            OpenList.Push(start);
            start.NodeTotalCost = 0.0f;
            start.EstimatedCost = HeuristicEstimateCost(start, goal);
            ClosedList = new PriorityQueue();
            AStarNode node = null;
            while (OpenList.Length != 0)
            {
                node = OpenList.First();
                //Check if the current node is the goal node  
                if (node.Position == goal.Position)
                {
                    return CalculatePath(node);
                }
                //Create an ArrayList to store the neighboring nodes  
                List<AStarNode> neighbours = new List<AStarNode>();
                GridManager.Instance.GetNeighbours(node, neighbours);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    AStarNode neighbourNode = neighbours[i] as AStarNode;
                    if (!ClosedList.Contains(neighbourNode))
                    {
                        float cost = HeuristicEstimateCost(node,
                            neighbourNode);
                        float totalCost = node.NodeTotalCost + cost;
                        float neighbourNodeEstCost = HeuristicEstimateCost(
                            neighbourNode, goal);
                        neighbourNode.NodeTotalCost = totalCost;
                        neighbourNode.Parent = node;
                        neighbourNode.EstimatedCost = totalCost +
                                                      neighbourNodeEstCost;
                        if (!OpenList.Contains(neighbourNode))
                        {
                            OpenList.Push(neighbourNode);
                        }
                    }
                }
                //Push the current node to the closed list  
                ClosedList.Push(node);
                //and remove it from openList  
                OpenList.Remove(node);
            }
            if (node.Position != goal.Position)
            {
                Debug.LogError("Goal Not Found");
                return null;
            }
            return CalculatePath(node);
        }

        private static List<AStarNode> CalculatePath(AStarNode node)
        {
            List<AStarNode> list = new List<AStarNode>();
            while (node != null)
            {
                list.Add(node);
                node = node.Parent;
            }
            list.Reverse();
            return list;
        }
    }
}