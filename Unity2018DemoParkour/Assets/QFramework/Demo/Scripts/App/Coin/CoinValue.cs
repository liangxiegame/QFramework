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

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 金币分值
	/// </summary>
	public class CoinValue : QMonoBehaviour, ICmd
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		private const float Duration = 0.8f;

		/// <summary>
		/// 重置颜色
		/// </summary>
		public void ResetCoinValue()
		{
			for (var i = 0; i < transform.childCount; i++)
			{
				var childTrans = transform.GetChild(i);
				childTrans.GetComponent<SpriteRenderer>().color = Color.white;
			}
		}

		/// <summary>
		/// 执行
		/// </summary>
		public void Execute()
		{
//		QTween.MoveBy(transform, Duration, Vector3.up * 2.0f);
//		QTween.ScaleTo(transform, Duration, new Vector3(1.2f, 1.2f, 0.0f));

			Invoke("FadeOutChildTrans", 0.3f);
		}


		private void FadeOutChildTrans()
		{
			for (var i = 0; i < transform.childCount; i++)
			{
//				var childTrans = transform.GetChild(i);

//			QTween.FadeOut(childTrans, 0.5f);
			}
		}

		void OnDespawned()
		{
			CancelInvoke();
		}
	}
}