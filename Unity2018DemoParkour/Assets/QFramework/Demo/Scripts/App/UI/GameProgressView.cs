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

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 白天是太阳,晚上是月亮应该
	/// </summary>

	public enum GameProgressState
	{
		Day,
		Night,
	}

	public class GameProgressView : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}


//		public UISlider slider;
//		public UISprite progress;
//		public UISprite leftIcon;
//		public UISprite rightIcon;
//		public UISprite leftNumber;
//		public UISprite rightNumber;

		int m_nRemainSun  = 7;
		int m_nRemainMoon = 7;

		public int RemainSun
		{
			get { return m_nRemainSun; }
			set
			{
				m_nRemainSun = value;
				UpdateNumbers();
			}
		}

		public int RemainMoon
		{
			get { return m_nRemainMoon; }
			set
			{
				m_nRemainMoon = value;
				UpdateNumbers();
			}
		}

		void UpdateNumbers()
		{
			switch (curState)
			{
				case GameProgressState.Day:
//					leftNumber.spriteName = "sun_" + m_nRemainSun;
//					rightNumber.spriteName = "sun_" + m_nRemainMoon;
					break;
				case GameProgressState.Night:
//					leftNumber.spriteName = "sun_" + m_nRemainMoon;
//					rightNumber.spriteName = "sun_" + m_nRemainSun;
					break;
			}
		}

		public GameProgressState curState = GameProgressState.Day;


		void Awake()
		{
//			slider = transform.GetComponent<UISlider>();
//			progress = transform.Find("Foreground").GetComponent<UISprite>();
//			leftIcon = transform.Find("left_icon").GetComponent<UISprite>();
//			rightIcon = transform.Find("right_icon").GetComponent<UISprite>();
//			leftNumber = transform.Find("left_number").GetComponent<UISprite>();
//			rightNumber = transform.Find("right_number").GetComponent<UISprite>();
		}

		void Start()
		{
			ResetView();
		}


		public void Day()
		{
			curState = GameProgressState.Day;
			RemainSun = m_nRemainSun;
			RemainMoon = m_nRemainMoon;
//			leftIcon.spriteName = "sun_icon";
//			rightIcon.spriteName = "moon_icon";
//			progress.spriteName = "day_progress";

//			leftIcon.MakePixelPerfect();
//			rightIcon.MakePixelPerfect();
		}

		public void Night()
		{
			curState = GameProgressState.Night;
			RemainSun = m_nRemainSun;
			RemainMoon = m_nRemainMoon;
//			leftIcon.spriteName = "moon_icon";
//			rightIcon.spriteName = "sun_icon";
//			progress.spriteName = "night_progress";

//			leftIcon.MakePixelPerfect();
//			rightIcon.MakePixelPerfect();
		}

		// 更新显示
		public void UpdateView()
		{
//			slider.value = InfoManager.Instance.gameInfo.curMeter;
		}

		public void ResetView()
		{
			Day();
			RemainSun = 7;
			RemainMoon = 7;
			UpdateView();
		}
	}
}