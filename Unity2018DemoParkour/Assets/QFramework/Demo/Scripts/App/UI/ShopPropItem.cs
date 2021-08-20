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
using QFramework;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 商店 项
	/// </summary>
	public class ShopPropItem : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		public QMonoBehaviour LevelLeft;
		public QMonoBehaviour LevelRight;
		public QMonoBehaviour LevelMiddle;

//		public UILabel descriptionLabel;

//		public UIButton    btn;
//		public NumberLabel priceLabel;

		public string Name;

		/// <summary>
		/// 获取引用
		/// </summary>
		void Awake()
		{
//			btn = transform.Find("btn_buy").GetComponent<UIButton>();
//			priceLabel = transform.Find("btn_buy").GetComponent<NumberLabel>();
//			descriptionLabel = transform.Find("description").GetComponent<UILabel>();

			LevelLeft = transform.Find("level_left").GetComponent<QMonoBehaviour>();
			LevelMiddle = transform.Find("level_middle").GetComponent<QMonoBehaviour>();
			LevelRight = transform.Find("level_right").GetComponent<QMonoBehaviour>();

			Name = gameObject.name;

			// 在这里处理按钮点击事件
//			btn.onClick.Add(new EventDelegate(delegate
//			{
//				if (InfoManager.Instance.gameInfo.coin - priceLabel.Number > 0)
//				{
//					InfoManager.Instance.gameInfo.coin -= priceLabel.Number;
//					InfoManager.Instance.SetLevel(name, InfoManager.Instance.GetLevel(name) + 1);
//				if (GameManager.Instance.homeCtrl)
//				{
//					GameManager.Instance.homeCtrl.foreWnd.UpdateCoin();
//				} else {
//					GameManager.Instance.uiCtrl.foreWnd.UpdateCoin();
//
//				}

//					UpdateView();
//				}
//			}));
		}

		void Start()
		{
			SetupView();
			UpdateView();
		}

		// 设置
		void SetupView()
		{
			UpdateDescription();
		}

		void UpdateView()
		{
			UpdateLevel(InfoManager.Instance.GetLevel(Name));

			if (InfoManager.Instance.GetLevel(Name) == 3)
			{
//				priceLabel.gameObject.SetActive(false);
				UpdateDescription();
				return;
			}

			Debug.LogWarning("level:" + InfoManager.Instance.GetLevel(Name));

			UpdatePrice();

			UpdateDescription();
		}

		/// <summary>
		/// 更新价格
		/// </summary>
		void UpdatePrice()
		{
//		int level = InfoManager.Instance.GetLevel(name);
//		int price = QConfigManager.Instance.GetValue("prop", name, "price" + level).IntValue;
//		Debug.LogWarning("@@@price:" + price);
//		priceLabel.SetNum(price);
		}

		/// <summary>
		/// 更新简述
		/// </summary>
		void UpdateDescription()
		{
//		string textPattern = QConfigManager.Instance.GetValue("prop", name, "description").StrValue;
//		int level = InfoManager.Instance.GetLevel(name);
//		float time = QFramework.QConfigManager.Instance.GetValue("prop", name, "time" + level).FloatValue;
//
//		// 特殊处理
//		if (name == "fruit")
//		{
//			descriptionLabel.text = string.Format(textPattern, time * 100 - 100);
//		}
//		else
//		{
//			descriptionLabel.text = string.Format(textPattern, time);
//
//		}
		}

		/// <summary>
		/// 更新等级
		/// </summary>
		public void UpdateLevel(int num)
		{
			switch (num)
			{
				case 0:
					LevelLeft.Hide();
					LevelMiddle.Hide();
					LevelRight.Hide();
					break;
				case 1:
					LevelLeft.Show();
					LevelMiddle.Hide();
					LevelRight.Hide();
					break;
				case 2:
					LevelLeft.Show();
					LevelMiddle.Show();
					LevelRight.Hide();
					break;
				case 3:
					LevelLeft.Show();
					LevelMiddle.Show();
					LevelRight.Show();
					break;
				default:
					break;
			}
		}
	}
}