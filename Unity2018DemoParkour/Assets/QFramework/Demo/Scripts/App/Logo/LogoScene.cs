/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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
using System.Collections;

namespace QFramework.PlatformRunner
{
	public class LogoScene : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

//	private Transform mLogoTrans;

		private bool m_bLaunchDone = false;

		public bool LaunchDone
		{
			get { return m_bLaunchDone; }
		}

		void Awake()
		{
//		mLogoTrans = transform.Find ("logo");
//		logoTrans.GetComponent<UITexture> ().color = Color.black; // 默认为黑色
		}

		public void LaunchLogo()
		{
//		QTween.TintTo (logoTrans, 1.0f, Color.black, Color.white);
			// 计时1s
			StartCoroutine(Launching());
		}

		IEnumerator Launching()
		{
			yield return new WaitForSeconds(1.1f);
			m_bLaunchDone = true;
		}


		public void TintBlack()
		{
//		QTween.TintTo (logoTrans, 1.0f, Color.white, Color.black);
		}
	}
}