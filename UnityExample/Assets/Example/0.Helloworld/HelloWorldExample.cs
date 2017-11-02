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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example 
{
	/// <summary>
	/// Hello World
	/// </summary>
	public class HelloWorldExample : MonoBehaviour 
	{
		/* 
		 * Hello World不知道写一些什么,但是Hello World很重要。所以介绍一个比较酷的特性吧
		 */
		void Start () 
		{
			this
			.Position(Vector2.zero)
			.LocalScaleIdentity()
			.Sequence()
			.Event(()=>Log.I("感谢使用QFramework"))
			.Delay(0.5f)
			.Event(()=>Log.I("延时了0.5秒"))
			.Event(()=>Log.I(transform.position))
			.Event(()=>Log.I(transform.localScale))
			.Begin()
			.DisposeWhenFinished()
			.OnDisposed(()=>Log.I("被销毁了"));
		}
	
		// Update is called once per frame
		void Update () 
		{
		
		}
	}
}