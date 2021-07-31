/****************************************************************************
 * Copyright (c) 2021.7 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using System;
using UnityEngine;

namespace QFramework
{
    /// <inheritdoc />
    /// <summary>
    /// 安枕执行节点
    /// </summary>
    [Serializable]
    [ActionGroup("ActionKit")]
    public class DelayFrameAction : ActionKitAction, IPoolable, IResetable
    {
        [SerializeField] public int FrameCount;

        public System.Action OnDelayFrameFinish { get; set; }

        public float CurrentSeconds { get; set; }

        public static DelayFrameAction Allocate(int frameCount, System.Action onDelayFrameFinish = null)
        {
            var retNode = SafeObjectPool<DelayFrameAction>.Instance.Allocate();
            retNode.FrameCount = frameCount;
            retNode.OnDelayFrameFinish = onDelayFrameFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }

        protected override void OnReset()
        {
            CurrentSeconds = 0.0f;
        }

        private int StartFrame;

        protected override void OnBegin()
        {
            base.OnBegin();

            StartFrame = Time.frameCount;
        }

        protected override void OnExecute(float dt)
        {
            Finished = Time.frameCount - StartFrame >= FrameCount;
            
            if (Finished && OnDelayFrameFinish != null)
            {
                OnDelayFrameFinish();
            }

        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayFrameAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            OnDelayFrameFinish = null;
            FrameCount = 0;
            Reset();
        }

        public bool IsRecycled { get; set; }
    }

    public static class DelayFrameActionExtensions
    {
        public static void DelayFrame<T>(this T selfBehaviour, int frameCount, System.Action delayFrameEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayFrameAction.Allocate(frameCount, delayFrameEvent));
        }
        
        public static void NextFrame<T>(this T selfBehaviour, System.Action nextFrameEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayFrameAction.Allocate(1, nextFrameEvent));
        }
        
        public static IActionChain DelayFrame(this IActionChain selfChain, int frameCount)
        {
            return selfChain.Append(DelayFrameAction.Allocate(frameCount));
        }
        
        public static IActionChain NextFrame(this IActionChain selfChain)
        {
            return selfChain.Append(DelayFrameAction.Allocate(1));
        }
    }
}