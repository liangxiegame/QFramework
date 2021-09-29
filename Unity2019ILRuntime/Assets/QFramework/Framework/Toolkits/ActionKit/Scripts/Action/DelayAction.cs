/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
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
    /// 延时执行节点
    /// </summary>
    [Serializable]
    [ActionGroup("ActionKit")]
    public class DelayAction : ActionKitAction, IPoolable, IResetable
    {

        [SerializeField]
        public float DelayTime;
        
        public System.Action OnDelayFinish { get; set; }

        public float CurrentSeconds { get; set; }

        public static DelayAction Allocate(float delayTime, System.Action onDelayFinish = null)
        {
            var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
            retNode.DelayTime = delayTime;
            retNode.OnDelayFinish = onDelayFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }

        protected override void OnReset()
        {
            CurrentSeconds = 0.0f;
        }

        protected override void OnExecute(float dt)
        {
            CurrentSeconds += dt;
            Finished = CurrentSeconds >= DelayTime;
            if (Finished && OnDelayFinish != null)
            {
                OnDelayFinish();
            }
        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            OnDelayFinish = null;
            DelayTime = 0.0f;
            Reset();
        }

        public bool IsRecycled { get; set; }
    }

    public static class DelayActionExtensions
    {
        public static void Delay<T>(this T selfBehaviour, float seconds, System.Action delayEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayAction.Allocate(seconds, delayEvent));
        }
        
        public static IActionChain Delay(this IActionChain selfChain, float seconds)
        {
            return selfChain.Append(DelayAction.Allocate(seconds));
        }
    }
    
    
}