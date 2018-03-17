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

    public class TestAStar : MonoBehaviour
    {
        private Transform mStartPos, mEndPos;
        public AStarNode StartNode { get; set; }
        public AStarNode GoalNode { get; set; }
        public List<AStarNode> PathArray;
        GameObject mObjStartCube, mObjEndCube;

        private float mElapsedTime = 0.0f;

        //Interval time between pathfinding  
        public float mIntervalTime = 1.0f;

        void Start()
        {
            mObjStartCube = GameObject.FindGameObjectWithTag("Start");
            mObjEndCube = GameObject.FindGameObjectWithTag("End");
            PathArray = new List<AStarNode>();
            FindPath();
        }

        void Update()
        {
            mElapsedTime += Time.deltaTime;
            if (mElapsedTime >= mIntervalTime)
            {
                mElapsedTime = 0.0f;
                FindPath();
            }
        }

        void FindPath()
        {
            mStartPos = mObjStartCube.transform;
            mEndPos = mObjEndCube.transform;
            StartNode = new AStarNode(GridManager.Instance.GetGridCellCenter(
                GridManager.Instance.GetGridIndex(mStartPos.position)));
            GoalNode = new AStarNode(GridManager.Instance.GetGridCellCenter(
                GridManager.Instance.GetGridIndex(mEndPos.position)));
            PathArray = AStar.FindPath(StartNode, GoalNode);
        }

        private void OnDrawGizmos()
        {
            if (PathArray == null)
                return;
            if (PathArray.Count > 0)
            {
                int index = 1;
                foreach (AStarNode node in PathArray)
                {
                    if (index < PathArray.Count)
                    {
                        AStarNode nextNode = PathArray[index];
                        Debug.DrawLine(node.Position, nextNode.Position,
                            Color.green);
                        index++;
                    }
                }
            }
        }
    }
}