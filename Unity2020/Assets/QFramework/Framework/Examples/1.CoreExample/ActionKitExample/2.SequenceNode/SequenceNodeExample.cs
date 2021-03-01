/****************************************************************************
 * Copyright (c) 2017 liangxie
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

using UnityEngine;

namespace QFramework.Example
{
    public class SequenceNodeExample : MonoBehaviour
    {
        private void Start()
        {
            this.Sequence()
                .Delay(1.0f)
                .Event(() => Log.I("Sequence1 延时了 1s"))
                .Begin()
                .OnDisposed(() => { Log.I("Sequence1 destroyed"); });

            var sequenceNode2 = new SequenceNode(DelayAction.Allocate(1.5f));
            sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 1.5s")));
            sequenceNode2.Append(DelayAction.Allocate(0.5f));
            sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 2.0s")));

            this.ExecuteNode(sequenceNode2);

            /* 这种方式需要自己手动进行销毁
            sequenceNode2.Dispose();
            sequenceNode2 = null;
            */

            // 或者 OnDestroy 触发时进行销毁
            sequenceNode2.DisposeWhenGameObjectDestroyed(this);
        }

        void OnLoginSucceed()
        {
        }

        private SequenceNode mSequenceNodeNode3 = new SequenceNode(
            DelayAction.Allocate(3.0f),
            EventAction.Allocate(() => { Log.I("Sequence3 延时 3.0f"); }));

        private void Update()
        {
            if (mSequenceNodeNode3 != null && !mSequenceNodeNode3.Finished &&
                mSequenceNodeNode3.Execute(Time.deltaTime))
            {
                Log.I("SequenceNode3 执行完成");
            }
        }

        private void OnDestroy()
        {
            mSequenceNodeNode3.Dispose();
            mSequenceNodeNode3 = null;
        }
    }
}