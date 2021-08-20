/****************************************************************************
 * Copyright (c) 2018.3 ~ 10 liangxie
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

namespace QFramework.PlatformRunner
{
	public class Game : MonoBehaviour
	{
		private ResLoader mResLoader;
		
		public static Game Load()
		{
			var resLoader = ResLoader.Allocate();

            return resLoader.LoadSync<GameObject>("Game")
				.Instantiate()
				.GetComponent<Game>()
				.ApplySelfTo(selfScript => selfScript.mResLoader = resLoader)
				.Identity()
				.Init();
		}

		/// <summary>
		/// 各种模块控制器
		/// </summary>
		public BgCtrl mBgCtrl;
		public CameraCtrl CameraCtrl;
		public PlayerCtrl PlayerCtrl;

		private Game Init()
		{
			//mBgCtrl = transform.Find("Camera/BgLayer").GetComponent<BgCtrl>();
			//CameraCtrl = transform.Find("Camera").GetComponent<CameraCtrl>();
//			PlayerCtrl = transform.Find("Player").GetComponent<PlayerCtrl>();
			return this;
		}

		public void Unload()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}