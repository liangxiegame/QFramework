/****************************************************************************
 * Copyright (c) 2018.3 liangxie
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
	public enum BossState
	{
		None,
		Appearing,
		Died,
		Idle,
	}

	public class BossSun : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

		private Transform idleTrans;
		private Transform dieTrans;
		private BossState curState = BossState.None;

		void Awake()
		{
			idleTrans = transform.Find("Idle");
			dieTrans = transform.Find("Die");
			idleTrans.Hide();
			dieTrans.Hide();
		}

		// 3.5 2.4 1
		// to 3.5 1.5 1
		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("arrow"))
			{
				other.transform.parent = transform;
				other.GetComponent<Arrow>().FadeOut();
			}
		}

		public void Appear()
		{
			idleTrans.Show();
			dieTrans.Hide();

			curState = BossState.Appearing;

			if (StageModel.Instance.Theme == 3)
			{
				transform.localScale = Vector3.zero;
				transform.localPosition = new Vector3(3.6f, 2.5f, 1.0f);
//				QTween.ScaleTo(transform, 1.0f, Vector3.one);
//				QTween.MoveTo(transform, 1.0f, new Vector3(3.5f, 2.4f, 1.0f));
				GameManager.Instance.cameraCtrl.Idle();

			}
			else
			{
				transform.localPosition = new Vector3(0.37f, 7.1f, 1.0f);
//				QTween.MoveTo(transform, 1.0f, new Vector3(3.5f, 2.4f, 1.0f));
			}

			StartCoroutine(Idle());
		}



		IEnumerator Idle()
		{
			yield return new WaitForSeconds(1.0f);
			curState = BossState.Idle;
//			QTween.PingPong(QTween.MoveTo(transform, 1.0f, new Vector3(3.5f, 2.4f, 1.0f), new Vector3(3.5f, 1.5f, 1.0f)));
		}

		public void Died()
		{
			dieTrans.Show();
			idleTrans.Hide();
//			QTween.MoveBy(transform, 0.1f, Vector3.zero);

			curState = BossState.Died;

			this.Delay(0.5f, delegate
			{
				if (curState == BossState.None)
				{
					return;
				}

//				QTween.MoveTo(transform, 1.5f, Vector3.down * 6.0f);
			});
		}

		protected override void OnHide()
		{
			curState = BossState.None;
			transform.localPosition = new Vector3(0.37f, 7.1f, 1.0f);
		}
	}
}