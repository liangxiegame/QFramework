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

namespace QFramework.Example
{
	using UnityEngine;

	public class DelayNodeExample : MonoBehaviour
	{
		void Start()
		{
			this.Delay(1.0f, () =>
			{
				Log.I("延时 1s");
			});

			var delay2s = DelayAction.Allocate(2.0f, () => { Log.I("延时 2s"); });
			this.ExecuteNode(delay2s);
		}

		private DelayAction mDelay3s = DelayAction.Allocate(3.0f, () => { Log.I("延时 3s"); });

		private void Update()
		{
			if (mDelay3s != null && !mDelay3s.Finished && mDelay3s.Execute(Time.deltaTime))
			{
				Log.I("Delay3s 执行完成");
			}
		}
	}
}
