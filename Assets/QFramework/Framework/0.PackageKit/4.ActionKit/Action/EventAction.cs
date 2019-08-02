/****************************************************************************
 * Copyright (c) 2017 ~ 2018.8 liangxie
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

namespace QF.Action
{
    using System;
    
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class EventAction : NodeAction, IPoolable
    {
        private System.Action mOnExecuteEvent;

        /// <summary>
        /// TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static EventAction Allocate(params System.Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<EventAction>.Instance.Allocate();
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
            return retNode;
        }

        /// <summary>
        /// finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            if (mOnExecuteEvent != null)
            {
                mOnExecuteEvent.Invoke();
            }

            Finished = true;
        }
        
        protected override void OnDispose()
        {
            SafeObjectPool<EventAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            Reset();
            mOnExecuteEvent = null;
        }

        public bool IsRecycled { get; set; }
        
//        [Test]
//        public static void Test()
//        {
//            int a = 0;
//            var delayAction = Allocate(() => { a++; });
// 
//            while (!delayAction.Finished && delayAction.Execute(0.02f))
//            {
// 
//            }
// 
//            delayAction.Dispose();
// 
//            Assert.IsTrue(((IPoolable) delayAction).IsRecycled);
//            Assert.AreEqual(delayAction.OnBeganCallback, null);
//            Assert.AreEqual(delayAction.OnEndedCallback, null);
//            Assert.AreEqual(delayAction.OnDisposedCallback, null);
//            Assert.AreEqual(delayAction.OnDisposedCallback, null);
//            Assert.AreEqual(a, 1);
//            Assert.IsFalse(delayAction.mDisposed);
//        }
    }
}