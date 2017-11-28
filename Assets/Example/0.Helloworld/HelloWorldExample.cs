/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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
using UnityEngine.UI;

namespace QFramework.Example
{
	/// <summary>
	/// Hello World
	/// </summary>
	public class HelloWorldExample : MonoBehaviour
	{
		/* 
		 * Hello World不知道写一些什么,但是Hello World很重要。所以介绍一个比较酷的特性。
		 */
		void Start()
		{
			this.Position(Vector2.zero)
				.LocalScaleIdentity()
				.Sequence()
				.Event(() => Log.I("Hello World"))
				.Delay(0.5f)
				.Event(() => Log.I("延时了0.5秒"))
				.Event(() => Log.I(transform.position))
				.Event(() => Log.I(transform.localScale))
				.Begin()
				.DisposeWhen(() => Time.realtimeSinceStartup > 10.0f)
				.OnDisposed(() => Log.I("被销毁了"));
		}

		/* 以上代码翻译成协程就是如下代码
		IEnumerator Start()
		{
			transform.position = Vector2.zero;
			transform.localScale = Vector3.one;
			yield return null;
			Log.I("Hello World");
			yield return new WaitForSeconds(0.5f);
			Log.I("延时了0.5秒");
			yield return null;
			Log.I(transform.position);
			yield return null;
			Log.I(transform.localScale);
			yield return null;
			Log.I("被销毁了");
		}
		*/

		/// <summary>
		/// 测试图片
		/// </summary>
		[SerializeField] private Image mImage;
		
		void Awake()
		{
			int countDown = 5;
			this.Repeat()
				.Delay(1.0f)
				.Event(()=>Log.I("Count Down:{0}",--countDown))
				.Begin()
				.DisposeWhen(() => countDown == 0)
				.OnDisposed(() => Log.I("On Disposed"));

			this.Position(Vector3.one)			// this.transform.position = Vector3.one
				.LocalScale(1.0f)				// this.transform.localScale = Vector3.one
				.Rotation(Quaternion.identity); // this.transform.rotation = Quaternion.identity

			mImage.LocalPosition(Vector3.back)
				.ColorAlpha(0.0f)
				.Hide();
		}
	}
}