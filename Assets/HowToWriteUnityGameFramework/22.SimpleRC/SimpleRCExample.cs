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

namespace QFramework.Example
{
	class Light
	{
		public void Open()
		{
			Log.I("灯打开了");
		}

		public void Close()
		{
			Log.I("灯关闭了");
		}
	}

	class Room : SimpleRC
	{
		private Light mLight = new Light();
		
		public void EnterPeople()
		{
			if (RefCount == 0)
			{
				mLight.Open();
			}

			Retain();
			
			Log.I("一个人走进房间,房间里当前有{0}个人",RefCount);
		}

		public void LeavePeople()
		{
			// 当前还没走出，所以输出的时候先减1
			Log.I("一个人走出房间,房间里当前有{0}个人", RefCount - 1);

			// 这里才真正的走出了
			Release();
		}

		protected override void OnZeroRef()
		{
			mLight.Close();
		}
	}
	
	public class SimpleRCExample : MonoBehaviour
	{
		
		// Use this for initialization
		void Start()
		{
			var room = new Room();
			room.EnterPeople();
			room.EnterPeople();
			room.EnterPeople();
			
			room.LeavePeople();
			room.LeavePeople();
			room.LeavePeople();
			
			room.EnterPeople();
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}