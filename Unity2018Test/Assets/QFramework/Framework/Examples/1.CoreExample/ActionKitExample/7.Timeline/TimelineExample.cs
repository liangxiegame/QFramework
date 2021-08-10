/****************************************************************************
 * Copyright (c) 2021.3 liangxie
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
    public class TimelineExample : MonoBehaviour
    {
        void Start()
        {
            var timelineNode = new Timeline();

            // 第一秒输出 HelloWorld
            timelineNode.Append(1.0f, EventAction.Allocate(() => Debug.Log("HelloWorld")));

            // 第二秒输出 延时了 2 秒
            timelineNode.Append(2.0f, EventAction.Allocate(() => Debug.Log("延时了 2 秒")));

            // 第三秒发送 一个事件
            timelineNode.Append(3.0f, KeyEventAction.Allocate("someEventA", timelineNode));

            // 第四秒发送 一个事件
            timelineNode.Append(4.0f, KeyEventAction.Allocate("someEventB", timelineNode));

            // 监听 timeline 的 key 事件
            timelineNode.OnKeyEventsReceivedCallback = keyEvent => Debug.Log(keyEvent);

            // 执行 timeline
            this.ExecuteNode(timelineNode);
        }
    }
}