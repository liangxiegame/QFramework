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
    using UnityEngine;
    
    public static class IExecuteNodeChainExtention
    {
        public static IExecuteNodeChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count);
            retNodeChain.Executer = selfbehaviour;
            // dispose when distroyed
            retNodeChain.DisposeWhenGameObjDestroyed();
            return retNodeChain;
        }
        
        public static IExecuteNodeChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain();
            retNodeChain.Executer = selfbehaviour;
            retNodeChain.DisposeWhenGameObjDestroyed();
            return retNodeChain;
        }
        
        public static IExecuteNodeChain Delay(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayNode.Allocate(seconds));
        }
        
        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IExecuteNodeChain Wait(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayNode.Allocate(seconds));
        }

        public static IExecuteNodeChain Event(this IExecuteNodeChain selfChain,params Action[] onEvents)
        {
            return selfChain.Append(EventNode.Allocate(onEvents));
        }

        public static IExecuteNodeChain Until(this IExecuteNodeChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilNode.Allocate(condition));
        }
    }
}