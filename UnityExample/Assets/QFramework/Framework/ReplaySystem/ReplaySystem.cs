/****************************************************************************
 * Copyright (c) 2017 liangxie
 *
 * 参考:https://github.com/IronWarrior/UnityReplaySystem
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
    using UnityEngine;


    /// <summary>
    /// Node类型,UI输入和MouseDown类型, Back
    /// </summary>
    public class ReplaySystem : QMonoSingleton<ReplaySystem>
    {
        private float mStartTime = 0.0f;
        private bool mRecording = false;
        private bool mReplaying = false;
        private IDisposable mUpdatingObserver = null;
        private IDisposable mReplayingObserver = null;

        public static bool Recording
        {
            get { return Instance.mRecording; }
        }

        public static void RecordStart()
        {
            Instance.mStartTime = 0.0f;
            Instance.mRecording = true;
            Instance.mReplayTimelineNode.TimelineQueue.Clear();
            Instance.mUpdatingObserver = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Instance.mRecording)
                {
                    Instance.mStartTime += Time.deltaTime;
                }
            });
        }

        public static void RecordNode(IExecuteNode node)
        {
            if (Instance.mRecording)
            {
                Instance.mReplayTimelineNode.Append(Instance.mStartTime, node);
            }
            else
            {
                Log.E("is not recording when append");
            }
        }

        private TimelineNode mReplayTimelineNode = new TimelineNode();

        public static void RecordStop()
        {
            Instance.mRecording = false;
            Instance.mUpdatingObserver.Dispose();
            Instance.mUpdatingObserver = null;
        }

        public static void PlayRecord(Action onReplayFinished)
        {
            Instance.mReplaying = true;
            Instance.mReplayTimelineNode.Reset();
            Instance.mReplayingObserver = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Instance.mReplaying && Instance.mReplayTimelineNode.Execute(Time.deltaTime))
                {
                    onReplayFinished.InvokeGracefully();
                    ReplayStop();
                }
            });
        }

        public static void ReplayStop()
        {
            Instance.mReplaying = false;
            Instance.mReplayingObserver.Dispose();
            Instance.mReplayingObserver = null;
        }
    }
}