/****************************************************************************
 * Copyright (c) 2018.7 ~ 2021.8 liangxie
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
	public class UsingProps : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		private readonly Transform[] mPropIcon  = new Transform[3];
		private readonly bool[]      mPropUsing = new bool[3] {false, false, false};
		string[]                     propName   = new string[3];

		Coroutine[] propCoroutine = new Coroutine[3] {null, null, null};
//	NumberLabel[]                propTime      = new NumberLabel[3];

		void Awake()
		{
			mPropIcon[0] = transform.Find("prop1");
			mPropIcon[1] = transform.Find("prop2");
			mPropIcon[2] = transform.Find("prop3");

			for (var i = 0; i < 3; i++)
			{
//			propTime[i] = mPropIcon[i].Find("number").GetComponent<NumberLabel>();
			}
		}

		private void Start()
		{
			mPropIcon[0].gameObject.SetActive(false);
			mPropIcon[1].gameObject.SetActive(false);
			mPropIcon[2].gameObject.SetActive(false);
		}

		public void ShowProp(string name, int time)
		{
			// 如果已经在使用
			for (int i = 0; i < 3; i++)
			{
				if (propName[i].CompareTo(name) == 0)
				{
					ShowPropWithIndex(i, name, time);
					return;
				}
			}

			// 如果没有使用
			for (int i = 0; i < 3; i++)
			{
				if (!mPropUsing[i])
				{
					ShowPropWithIndex(i, name, time);
					return;
				}
			}
		}

		IEnumerator TickTime(int index, int time)
		{
			int remainTime = time;
			while (remainTime > -1)
			{
//			propTime[index].SetNum(remainTime);
				remainTime--;
				yield return new WaitForSeconds(1.0f);
			}

			HideProp(index);
		}

		void ShowPropWithIndex(int index, string name, int time)
		{
			mPropUsing[index] = true;
//		var sprite = mPropIcon[index].GetComponent<UISprite>();
//		sprite.spriteName = name;

			mPropIcon[index].Show();
//		sprite.MakePixelPerfect();

			propName[index] = name;
//		QTween.FadeIn(mPropIcon[index], 0.0f);

			if (propCoroutine[index] != null)
			{
				StopCoroutine(propCoroutine[index]);
				propCoroutine[index] = null;
			}

			propCoroutine[index] = StartCoroutine(TickTime(index, time));
		}

		public void BlinkProp(string name)
		{
			for (var i = 0; i < 3; i++)
			{
				if (name.CompareTo(propName[i]) == 0)
				{
//				QTween.Blink(mPropIcon[i], 0.3f);
					return;
				}
			}
		}

		void HideProp(int index)
		{
			mPropIcon[index].Hide();
			mPropUsing[index] = false;
			propName[index] = "";
		}

		public void HideProp(string name)
		{
			for (int i = 0; i < 3; i++)
			{
				if (name.CompareTo(propName[i]) == 0)
				{
					HideProp(i);
					return;
				}
			}
		}

		// 重置
		public void ResetView()
		{
			for (var i = 0; i < 3; i++)
			{
				mPropIcon[i].Hide();
				mPropUsing[i] = false;
				propName[i] = "";
			}
		}
	}
}