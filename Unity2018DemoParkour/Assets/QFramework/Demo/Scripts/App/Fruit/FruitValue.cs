/****************************************************************************
 * Copyright (c) 2018.3 ~ 7 liangxie
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
	/// 水果的分数值
	/// </summary>
	public class FruitValue : QMonoBehaviour, ICmd
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		public UGUINumberLabel number;
		public Transform       plusTrans;
		public Transform       minusTrans;

		private const float duration = 0.8f;

		public void Awake()
		{
			number = transform.Find("Number").GetComponent<UGUINumberLabel>();
			plusTrans = transform.Find("get_gold_num+");
			minusTrans = transform.Find("get_gold_num-");

			Plus();
		}

		/// <summary>
		/// 重置颜色
		/// </summary>
		public void ResetFruitValue()
		{
			for (var i = 0; i < transform.childCount; i++)
			{
				var childTrans = transform.GetChild(i);
				if (childTrans.name.CompareTo("Number") != 0)
				{
					childTrans.GetComponent<SpriteRenderer>().color = Color.white;
				}
			}
		}


		public void Plus()
		{
			minusTrans.Hide();
			plusTrans.Show();
		}

		public void Minus()
		{
			minusTrans.Show();
			plusTrans.Hide();
		}

		/// <summary>
		/// 执行
		/// </summary>
		public void Execute()
		{
//			QTween.MoveBy(transform, duration, Vector3.up * 2.0f);
//			QTween.ScaleTo(transform, duration, new Vector3(1.2f, 1.2f, 0.0f));

			Invoke("FadeOutChildTrans", 0.3f);

			Invoke("DespawnNumber", 0.8f);
		}

		private void FadeOutChildTrans()
		{
			for (var i = 0; i < transform.childCount; i++)
			{
				var childTrans = transform.GetChild(i);
				if (childTrans.name.CompareTo("Number") != 0)
				{
//					QTween.FadeOut(childTrans, 0.5f);
				}
			}

			number.FadeOut();
		}
	}
}