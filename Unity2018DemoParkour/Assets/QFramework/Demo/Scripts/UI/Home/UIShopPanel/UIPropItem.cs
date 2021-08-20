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
using UnityEngine.UI;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 道具项目
	/// </summary>
	public class UIPropItem : MonoBehaviour ,IController
	{

		[SerializeField] Text            mDescriptionText;
		[SerializeField] GameObject      mLevelLeft;
		[SerializeField] GameObject      mLevelMiddle;
		[SerializeField] GameObject      mLevelRight;
		[SerializeField] Button          mBtnBuy;
		[SerializeField] UGUINumberLabel mPriceLabel;
		[SerializeField] string          mPropName;

		void Awake()
		{
			mBtnBuy.onClick.AddListener(delegate
			{

				if (this.GetModel<IGameModel>().Coin.Value - mPriceLabel.Number > 0)
				{
					this.GetModel<IGameModel>().Coin.Value -= mPriceLabel.Number;
					InfoManager.Instance.SetLevel(mPropName, InfoManager.Instance.GetLevel(mPropName) + 1);

//				if (GameManager.Instance.homeCtrl)
//				{
//					GameManager.Instance.homeCtrl.foreWnd.UpdateCoin();
//				} else {
//					GameManager.Instance.uiCtrl.foreWnd.UpdateCoin();
//				}

//				QUGUIMgr.Instance.GetUIBehaviour<UIGoldLabel>(UIName.GoldLabel).Show();

					UpdateView();
				}
			});
		}

		void Start()
		{
//		mPriceLabel.SetNum (1000);

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
			UpdateLevel(InfoManager.Instance.GetLevel(mPropName));

			if (InfoManager.Instance.GetLevel(mPropName) == 3)
			{
				mPriceLabel.gameObject.SetActive(false);
				UpdateDescription();
				return;
			}

			Debug.LogWarning("level:" + InfoManager.Instance.GetLevel(mPropName));

			UpdatePrice();

			UpdateDescription();
		}

		/// <summary>
		/// 更新价格
		/// </summary>
		void UpdatePrice()
		{
//		int level = InfoManager.Instance.GetLevel (mPropName);
//		int price = QConfigManager.Instance.GetValue("prop",mPropName,"price" + level).IntValue;
//		Debug.LogWarning ("@@@price:" + price);
//		mPriceLabel.SetNum(price);
		}

		/// <summary>
		/// 更新简述
		/// </summary>
		void UpdateDescription()
		{
//		string textPattern = QConfigManager.Instance.GetValue ("prop", mPropName, "description").StrValue;
//		int level = InfoManager.Instance.GetLevel (mPropName);
//		float time = QFramework.QConfigManager.Instance.GetValue ("prop", mPropName, "time" + level).FloatValue;
//
//		// 特殊处理
//		if (mPropName == "fruit") {
//			mDescriptionText.text = string.Format (textPattern,time * 100 - 100);
//		} else {
//			mDescriptionText.text = string.Format (textPattern,time);
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
					mLevelLeft.SetActive(false);
					mLevelMiddle.SetActive(false);
					mLevelRight.SetActive(false);
					break;
				case 1:
					mLevelLeft.SetActive(true);
					mLevelMiddle.SetActive(false);
					mLevelRight.SetActive(false);
					break;
				case 2:
					mLevelLeft.SetActive(true);
					mLevelMiddle.SetActive(true);
					mLevelRight.SetActive(false);
					break;
				case 3:
					mLevelLeft.SetActive(true);
					mLevelMiddle.SetActive(true);
					mLevelRight.SetActive(true);
					break;
				default:
					break;
			}
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}