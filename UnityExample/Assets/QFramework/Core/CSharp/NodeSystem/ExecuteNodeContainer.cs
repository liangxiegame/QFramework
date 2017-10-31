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
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// 执行节点容器
    /// 打算弃用,应该继承SequenceNode来替代。
    /// </summary>
    public class ProcessNodeContainer
    {
        #region Event
        public Action<float> OnExecuteScheduleEvent;
        public Action<string> OnExecuteTipsEvent;
        public Action OnExecuteContainerBeginEvent;
        public Action OnExecuteContainerEndEvent;
        #endregion

        #region 属性&字段

        private List<ProcessNode> mNodeList;
        private int mCurrentIndex;
        private ProcessNode mCurrentNode;

        private float mTotalSchedule = 0;

        #endregion

        public float TotalSchedule
        {
            get { return mTotalSchedule; }
        }

        public ProcessNode CurrentNode
        {
            get { return mCurrentNode; }
        }

        public void Append(ProcessNode item)
        {
            if (mNodeList == null)
            {
                mNodeList = new List<ProcessNode>();
                mCurrentIndex = -1;
            }

            mNodeList.Add(item);
        }

        public void Begin()
        {
            mCurrentIndex = -1;
            MoveToNextUpdateFunc();
        }

        public void Execute(float dt)
        {
            if (mCurrentNode != null)
            {
                mCurrentNode.Execute(dt);

                float schedule = mCurrentNode.Progress;

                mTotalSchedule = mCurrentIndex * (1.0f / mNodeList.Count) + schedule / mNodeList.Count;

                if (OnExecuteScheduleEvent != null)
                {
                    OnExecuteScheduleEvent(mTotalSchedule);
                }

                if (mCurrentNode.Finished)
                {
                    MoveToNextUpdateFunc();
                }
            }
        }

        private void MoveToNextUpdateFunc()
        {
            ++mCurrentIndex;
            if (mCurrentIndex >= mNodeList.Count)
            {
                mTotalSchedule = 1.0f;
                mCurrentNode = null;

                if (OnExecuteContainerEndEvent.InvokeGracefully())
                {
                    OnExecuteContainerEndEvent = null;
                }
            }
            else
            {
                mCurrentNode = mNodeList[mCurrentIndex];

                if (mCurrentIndex == 0)
                {
                    OnExecuteContainerBeginEvent.InvokeGracefully();
                }

                OnExecuteTipsEvent.InvokeGracefully(mCurrentNode.Tips);
            }
        }
    }
}