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
using System.Collections.Generic;
using UnityEngine.UI;

namespace QFramework.PlatformRunner
{
	public enum Align
	{
		Left,
		Middle,
		Right,
	}
	/// <summary>
	/// 数字的控件，默认是Aligh Right
	/// </summary>
	public partial class UGUINumberLabel : MonoBehaviour, IBind
	{
		public int Number { get; set; }

		/// <summary>
		/// 权重
		/// </summary>
		public List<Transform> numberTrans = new List<Transform>();

		public List<Vector3> numberPos = new List<Vector3>();

		public Sprite[] sprites;

		/// <summary>
		/// 默认右对齐
		/// </summary>
		public Align align = Align.Right;

		/// <summary>
		/// 对其的位置
		/// </summary>
		public Vector3 leftPos;

		public Vector3 rightPos;
		public Vector3 middlePos;


		public float itemWidth; // 元素的宽度
		public float itemSpacing; // 元素的间距
		public float itemDistance; // 元素之间的距离

		public int numLength; // 数字的长度 

		/// <summary>
		/// 前缀
		/// </summary>
		public string prefix;

		void Awake()
		{
			numberPos.Clear();
			foreach (var numberChild in numberTrans)
			{
				numberPos.Add(numberChild.localPosition);
			}

			// 初始化数据
			leftPos = numberPos[numberTrans.Count - 1];
			if (numberTrans.Count % 2 == 0)
			{
				middlePos = numberPos[numberTrans.Count / 2] * 0.5f + numberPos[numberTrans.Count - 1] * 0.5f;
			}
			else
			{
				middlePos = numberPos[numberTrans.Count / 2];
			}

			rightPos = numberPos[0];

			itemWidth = numberPos[0].x - numberPos[1].x;
			itemDistance = numberPos[0].x - numberPos[1].x;
			itemSpacing = numberPos[0].x - numberPos[1].x - itemWidth;

		}

		/// <summary>
		/// 更新排版 可以优化: 增加标记,判断减少计算。
		/// </summary>
		public void UpdateAlign()
		{

			switch (align)
			{
				case Align.Left:
					for (int i = numLength - 1; i >= 0; i--)
					{
						numberTrans[i].localPosition = leftPos + Vector3.right * (numLength - i - 1) * itemDistance;
					}

					break;
				case Align.Middle:

					if (numLength % 2 == 0)
					{
						// 偶数
						for (int i = numLength - 1; i >= 0; i--)
						{
							if (numberTrans[i] != null)
							{
								numberTrans[i].localPosition = middlePos + new Vector3((numLength / 2 - 0.5f - i) * itemDistance, 0, 0);
							}
						}
					}
					else
					{
						// 奇数
						for (int i = numLength - 1; i >= 0; i--)
						{
							if (numberTrans[i] != null)
							{
								numberTrans[i].localPosition = middlePos + new Vector3((numLength / 2 - i) * itemDistance, 0, 0);
							}
						}
					}

					break;
				case Align.Right:

					for (int i = numLength - 1; i >= 0; i--)
					{
						if (numberTrans[i] != null)
						{
							numberTrans[i].localPosition = rightPos - new Vector3(i * itemDistance, 0, 0);
						}
					}

					break;
				default:
					break;
			}
		}

		public void SetNum(int num)
		{
			Number = num;


			List<int> n = new List<int>();

			int tempLength = numberTrans.Count;

			for (int i = 0; i < numberTrans.Count; i++)
			{
				n.Add(num % 10);
				num /= 10;
			}

			bool firstBit = true;
			for (int i = numberTrans.Count - 1; i >= 0; i--)
			{
				if (firstBit && n[i] == 0 && i != 0)
				{
					numberTrans[i].gameObject.SetActive(false);
					tempLength--;
				}
				else
				{
					firstBit = false;
					var image = numberTrans[i].GetComponent<Image>();
					if (image != null)
					{
						image.sprite = sprites[n[i]];
						image.color = Color.white; // 重置alpha值
					}
					else
					{
						numberTrans[i].GetComponent<SpriteRenderer>().sprite = sprites[n[i]];
						numberTrans[i].GetComponent<SpriteRenderer>().color = Color.white;

					}

					numberTrans[i].SetParent(transform);
					numberTrans[i].gameObject.SetActive(true);
				}
			}

			numLength = tempLength;
			// 更新布局 只更新一次
			UpdateAlign();
		}


		/// <summary>
		/// 淡出效果
		/// </summary>
		public void FadeOut()
		{
			for (int i = 0; i < 5; i++)
			{
				if (numberTrans[i] != null)
				{
//					QTween.FadeOut(numberTrans[i], 0.5f);
				}
			}
		}

		public string ComponentName
		{
			get { return "UGUINumberLabel"; }
		}

		public string Comment { get; private set; }

		public Transform Transform
		{
			get { return transform; }
		}

		public BindType GetBindType()
		{
			return BindType.Element;
		}
	}
}