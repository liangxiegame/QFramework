/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
 * Copyright (c) 2018.10 liangxie
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

namespace QFramework
{
	#region 事件定义

	public enum TestEvent
	{
		Start,
		TestOne,
		End,
	}

	public enum TestEventB
	{
		Start = TestEvent.End, // 为了保证每个消息 Id 唯一，需要头尾相接
		TestB,
		End,
	}

	#endregion 事件定义

	public class EventReceiverExample : MonoBehaviour
	{
		void Start()
		{
			QEventSystem.RegisterEvent(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
					obj[0].LogInfo();
					break;
			}
		}

		private void OnDestroy()
		{
			QEventSystem.UnRegisterEvent(TestEvent.TestOne, OnEvent);
		}
	}
}