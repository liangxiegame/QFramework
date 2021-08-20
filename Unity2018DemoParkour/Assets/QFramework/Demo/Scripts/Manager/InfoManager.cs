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
using System.Collections.Generic;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 可变数据管理
	/// </summary>
	public class InfoManager : QMgrBehaviour, ISingleton,IController
	{
		public static InfoManager Instance
		{
			get { return MonoSingletonProperty<InfoManager>.Instance; }
		}

		public override int ManagerId
		{
			get { return QMgrID.PCConnectMobile; }
		}

		protected override void ProcessMsg(int eventKey, QMsg msg)
		{
			switch (msg.EventID)
			{
				case (int) InfoEvent.Reset:
					//随机生成1或者3的主题
					this.GetModel<IGameModel>().CurTheme.Value = Random.Range(1, 3);
					Time.timeScale = 1.0f;
					Debug.LogWarning("初始化游戏");

					break;

				case (int) InfoEvent.CoinAdd:
					this.GetModel<IGameModel>().Coin.Value += ((MsgWithInt) msg).Value;
					break;

				case (int) InfoEvent.SetTheme:
					this.GetModel<IGameModel>().CurTheme.Value  =((MsgWithInt) msg).Value;
					break;
			}
		}

		public void OnSingletonInit()
		{
			propInfos = PlayerPrefService.LoadPropInfos(new List<string>()
			{
				"magnetite",
				"big",
				"find",
				"protect",
				"auto",
				"goldX2",
				"fruitX2",
				"fruit"
			});

			foreach (KeyValuePair<string, PropInfo> pair in propInfos)
			{
				Debug.LogWarning("@@@@ " + pair.Key + ":" + pair.Value.level);
			}
			
			RegisterEvent(InfoEvent.Reset);
			RegisterEvent(InfoEvent.CoinAdd);
			RegisterEvent(InfoEvent.SetTheme);
			
			
		}
		
		public Dictionary<string, PropInfo> propInfos;

		/// <summary>
		/// 设置道具的等级
		/// </summary>
		public void SetLevel(string name, int level)
		{
			propInfos[name].level = level;
		}

		/// <summary>
		/// 更新等级
		/// </summary>
		public int GetLevel(string name)
		{
			return propInfos[name].level;
		}


		/// <summary>
		/// 保存玩家的信息
		/// </summary>
		public void SaveGameInfo()
		{
//			PlayerPrefService.SaveGameInfo(gameInfo);
		}

		/// <summary>
		/// 保存道具的等级信息
		/// </summary>
		public void SavePropInfos()
		{
			PlayerPrefService.SavePropInfos(propInfos);
		}

		/// <summary>
		/// 保存所有的数据
		/// </summary>
		public void SaveAllInfos()
		{
			SaveGameInfo();
			SavePropInfos();
		}

		private void OnApplicationQuit()
		{
			SaveAllInfos();
			Framework.Instance.OnApplicationQuitEvent -= OnApplicationQuit;
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}