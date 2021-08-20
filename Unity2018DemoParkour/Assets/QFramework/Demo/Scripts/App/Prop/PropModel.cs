/****************************************************************************
 * Copyright (c) liangxiegame Under MIT License
 *
 * 2018.3 ~ 2021.8 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using System.Collections;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 道具状态的记录
	/// </summary>
	public class PropModel : Singleton<PropModel>
	{

		public class PropDataDefine
		{
			public int     id;
			public string  name;
			public int[]   price = new int[3];
			public float[] time  = new float[4];
			public string  description;

			public PropDataDefine(int id, string name, int price0, int price1, int price2, float time0, float time1, float time2,
				float time3, string description)
			{
				this.id = id;
				this.name = name;
				this.price[0] = price0;
				this.price[1] = price1;
				this.price[2] = price2;
				this.time[0] = time0;
				this.time[1] = time1;
				this.time[2] = time2;
				this.time[3] = time3;
				this.description = description;
			}
		}

//	public Dictionary<string,PropDataDefine> PropDefines = new Dictionary<string, PropDataDefine>() ;
//	{
//		{"magnetite",new PropDataDefine(1,"magnetite",300,600,800,8.0f,12.0f,16.0f,20.0f,"可以用来吸没有碰到的附近的金币和水果。")},
//		{"big",new PropDataDefine(2,"big",300,600,800,8.0f,12.0f,16.0f,20.0f,"当捡到此道具大力神提及会变大,不用跳跃便可碰到金币和水果,此过程中跑到的障碍物不影响大力神的体力值。也不会死掉。但是掉坑里依然会死掉。")},
//		{"find",new PropDataDefine(3,"find",500,700,1000,8.0f,12.0f,16.0f,20.0f,"当捡到此道具,水果会变大,更容易被收集到。")},
//		{"protect",new PropDataDefine(4,"protect",500,700,1000,8.0f,12.0f,16.0f,20.0f,"捡到此道具,大力神碰到障碍物不回死掉,掉到坑里会死掉。有台阶挡住路,无法前进也会死掉。")},
//		{"auto",new PropDataDefine(5,"auto",600,800,1200,8.0f,12.0f,16.0f,20.0f,"玩家可以有8秒的放松时间,在这8秒中不操作仍然不会死,操作也不会有反应,属于机器自动游戏时间。")},
//		{"gold_x2",new PropDataDefine(6,"gold_x2",600,800,1200,8.0f,12.0f,16.0f,20.0f,"金币面值翻倍。")},
//		{"fruit_x2",new PropDataDefine(7,"fruit_x2",600,800,1200,8.0f,12.0f,16.0f,20.0f,"水果面值翻倍。")},
//		{"fruit",new PropDataDefine(8,"auto",700,900,1400,1.0f,1.1f,Mathf.Pow(1.1f,2.0f),Mathf.Pow(1.1f,3.0f),"针对所有水果,每升级一次提高单个水果10%的体力值。")},
//	};

		private PropModel()
		{
		}


		/// <summary>
		/// 初始化数据
		/// </summary>
		public void InitModel()
		{


			ResetModel();
		}

		public void ResetModel()
		{
			if (autoCoroutine != null)
			{
				GameManager.Instance.StopCoroutine(autoCoroutine);
				autoCoroutine = null;
			}

			if (bigCoroutine != null)
			{
				GameManager.Instance.StopCoroutine(bigCoroutine);
				bigCoroutine = null;
			}

			if (fruitX2Coroutine != null)
			{
				GameManager.Instance.StopCoroutine(fruitX2Coroutine);
				fruitX2Coroutine = null;
			}

			if (goldX2Coroutine != null)
			{
				GameManager.Instance.StopCoroutine(goldX2Coroutine);
				goldX2Coroutine = null;
			}

			if (protectCoroutine != null)
			{
				GameManager.Instance.StopCoroutine(protectCoroutine);
				protectCoroutine = null;
			}

			if (magnetiteCoroutine != null)
			{
				GameManager.Instance.StopCoroutine(magnetiteCoroutine);
				magnetiteCoroutine = null;
			}

			if (findCoroutine != null)
			{
				GameManager.Instance.StopCoroutine(findCoroutine);
				findCoroutine = null;
			}

			prop_big_on = false;
			prop_find_on = false;
			prop_auto_on = false;
			prop_fruit_x2_on = false;
			prop_gold_x2_on = false;
			prop_magnetite_on = false;
			prop_time_extra = false;
			prop_protect_on = false;

			GameManager.Instance.stageCtrl.AutoBlock.Hide();
		}

		/// <summary>
		/// 各个道具的状态
		/// </summary>
		public bool prop_big_on = false;

		public bool prop_find_on      = false;
		public bool prop_auto_on      = false;
		public bool prop_fruit_x2_on  = false;
		public bool prop_gold_x2_on   = false;
		public bool prop_magnetite_on = false;
		public bool prop_time_extra   = false;
		public bool prop_protect_on   = false;

		Coroutine autoCoroutine      = null;
		Coroutine bigCoroutine       = null;
		Coroutine fruitX2Coroutine   = null;
		Coroutine goldX2Coroutine    = null;
		Coroutine protectCoroutine   = null;
		Coroutine magnetiteCoroutine = null;
		Coroutine findCoroutine      = null;

		/// <summary>
		/// 处理道具事件
		/// </summary>
		/// <param name="name">道具的tag.</param>
		public void HandleGetPropEvent(string name)
		{
			switch (name)
			{
				case "prop_auto": // 不用操作
					if (autoCoroutine != null)
					{
						GameManager.Instance.StopCoroutine(autoCoroutine);
						autoCoroutine = null;
					}

					autoCoroutine = GameManager.Instance.StartCoroutine(AutoExecute());

					break;
				case "prop_big":
					if (bigCoroutine != null)
					{
						GameManager.Instance.StopCoroutine(bigCoroutine);
						bigCoroutine = null;
					}

					bigCoroutine = GameManager.Instance.StartCoroutine(BigExecute());
					break;
				case "prop_fruit_x2":
					if (fruitX2Coroutine != null)
					{
						GameManager.Instance.StopCoroutine(fruitX2Coroutine);
						fruitX2Coroutine = null;
					}

					fruitX2Coroutine = GameManager.Instance.StartCoroutine(FruitX2Execute());
					break;
				case "prop_gold_x2":
					if (goldX2Coroutine != null)
					{
						GameManager.Instance.StopCoroutine(goldX2Coroutine);
						goldX2Coroutine = null;
					}

					goldX2Coroutine = GameManager.Instance.StartCoroutine(GoldX2Execute());
					break;

				case "prop_protect":
					if (protectCoroutine != null)
					{
						GameManager.Instance.StopCoroutine(protectCoroutine);
						protectCoroutine = null;
					}

					protectCoroutine = GameManager.Instance.StartCoroutine(ProtectExecute());
					break;
				case "prop_magnetite":
					if (magnetiteCoroutine != null)
					{
						GameManager.Instance.StopCoroutine(magnetiteCoroutine);
						magnetiteCoroutine = null;
					}

					magnetiteCoroutine = GameManager.Instance.StartCoroutine(MagnetiteExecute());
					break;

				default:
					break;
			}
		}


		/// <summary>
		/// 随机生成道具的种类,相应的道具逻辑可以写在这里
		/// </summary>
		/// <returns>The property data.</returns>
		public int GenPropData()
		{
			int retValue = Random.Range(STAGE.PROP_BEGIN, STAGE.PROP_END);
			// 测试
//		retValue = STAGE.PA;	// 自动 (槟郎)
//		retValue = STAGE.PB;	// 变大 
//		retValue = STAGE.PF2;   // 水果值双倍
//		retValue = STAGE.PG2;   // 金币值双倍
//		retValue = STAGE.PP;	// 保护作用
//		retValue = STAGE.PM;    // 吸铁石
//		retValue = STAGE.PF;    // 水果变大
			return retValue;
		}

		/// <summary>
		/// 槟郎执行
		/// </summary>
		/// <returns>The execute.</returns>
		public IEnumerator AutoExecute()
		{
//			PlayerCtrl playerCtrl = GameManager.Instance.playerCtrl;
			StageCtrl stageCtrl = GameManager.Instance.stageCtrl;
//		UICtrl uiCtrl = GameManager.Instance.uiCtrl;

			int level = InfoManager.Instance.GetLevel("auto");
			float seconds = ConfigManager.Instance.GetPropConfigByName("auto").Times[level];

//			playerCtrl.AutoEffectEmmiter.Play(); // 开启特效

			// 使用增加时间比例
			prop_auto_on = true;

//			playerCtrl.BlinkEnd();
//		uiCtrl.ShowProp ("prop_auto",(int)seconds);
			GameManager.Instance.SendMsg(new QMsg((ushort) PlayerEvent.Land));
			stageCtrl.AutoBlock.SetActive(true);

			yield return new WaitForSeconds(seconds - 2.8f);

//			playerCtrl.BlinkBegin();
//		uiCtrl.BlinkProp ("prop_auto");
			yield return new WaitForSeconds(1.8f);

//			playerCtrl.BlinkEnd();
			prop_auto_on = false;
//		uiCtrl.HideProp ("prop_auto");
			yield return new WaitForSeconds(1.0f);

//			playerCtrl.AutoEffectEmmiter.Pause();; // 关闭特效
			GameManager.Instance.SendMsg(new MsgWithFloat((ushort) GameEvent.SetTimeScale, 1.0f)); // 改变的应该是时间

			yield return new WaitForSeconds(0.5f);
			stageCtrl.AutoBlock.SetActive(false);

			autoCoroutine = null;
		}

		/// <summary>
		/// 变大执行
		/// </summary>
		/// <returns>The execute.</returns>
		public IEnumerator BigExecute()
		{
//			PlayerCtrl playerCtrl = GameManager.Instance.playerCtrl;
			StageCtrl stageCtrl = GameManager.Instance.stageCtrl;
//		UICtrl uiCtrl = GameManager.Instance.uiCtrl;

			var level = InfoManager.Instance.GetLevel("big");
			// 道具持续的时间
			var seconds = ConfigManager.Instance.GetPropConfigByName("big").Times[level];

			if (prop_big_on)
			{
//				playerCtrl.BlinkEnd();

			}
			else
			{
				prop_big_on = true;

//				playerCtrl.BigReset();
//				playerCtrl.BigBegin();


			}

			// 更新
			stageCtrl.BigReset();

//		uiCtrl.ShowProp ("prop_big",(int)seconds);

			yield return new WaitForSeconds(seconds - 1.8f);

//			playerCtrl.BlinkBegin();
//		uiCtrl.BlinkProp ("prop_big");
			yield return new WaitForSeconds(1.8f);

//			playerCtrl.BlinkEnd();
//		uiCtrl.HideProp ("prop_big");
//			playerCtrl.BigEnd();
			prop_big_on = false;

			stageCtrl.BigReset();
			bigCoroutine = null;
		}

		/// <summary>
		/// 水果值双倍
		/// </summary>
		/// <returns>The x2 execute.</returns>
		public IEnumerator FruitX2Execute()
		{
//		UICtrl uiCtrl = GameManager.Instance.uiCtrl;
			int level = InfoManager.Instance.GetLevel("fruitX2");
			float seconds = ConfigManager.Instance.GetPropConfigByName("fruit_x2").Times[level];

			prop_fruit_x2_on = true;

//		uiCtrl.ShowProp ("prop_fruit_x2",(int)seconds);
			yield return new WaitForSeconds(seconds - 1.8f);
//		uiCtrl.BlinkProp ("prop_fruit_x2");
			yield return new WaitForSeconds(1.8f);
//		uiCtrl.HideProp ("prop_fruit_x2");
			prop_fruit_x2_on = false;
			fruitX2Coroutine = null;
		}

		/// <summary>
		/// 金币值双倍
		/// </summary>
		/// <returns>The x2 execute.</returns>
		public IEnumerator GoldX2Execute()
		{
			prop_gold_x2_on = true;

//		UICtrl uiCtrl = GameManager.Instance.uiCtrl;
			int level = InfoManager.Instance.GetLevel("goldX2");
			float seconds = ConfigManager.Instance.GetPropConfigByName("gold_x2").Times[level];
//		uiCtrl.ShowProp ("prop_gold_x2",(int)seconds);
			yield return new WaitForSeconds(seconds - 1.8f);
//		uiCtrl.BlinkProp ("prop_gold_x2");
			yield return new WaitForSeconds(1.8f);
//		uiCtrl.HideProp ("prop_gold_x2");
			prop_gold_x2_on = false;
			goldX2Coroutine = null;
		}

		/// <summary>
		/// 保护
		/// </summary>
		/// <returns>The execute.</returns>
		public IEnumerator ProtectExecute()
		{
//			PlayerCtrl playerCtrl = GameManager.Instance.playerCtrl;
//		UICtrl uiCtrl = GameManager.Instance.uiCtrl;
			int level = InfoManager.Instance.GetLevel("protect");

			float seconds = ConfigManager.Instance.GetPropConfigByName("protect").Times[level];

			if (prop_protect_on)
			{
//				playerCtrl.Protect.BlinkEnd();

			}
			else
			{
				prop_protect_on = true;

//				playerCtrl.Protect.Show();
			}

//		uiCtrl.ShowProp ("prop_protect",(int)seconds);
			yield return new WaitForSeconds(seconds - 1.8f);

//			playerCtrl.Protect.BlinkBegin();
//		uiCtrl.BlinkProp ("prop_protect");
			yield return new WaitForSeconds(1.8f);

//			playerCtrl.Protect.BlinkEnd();

//			playerCtrl.Protect.Hide();
			prop_protect_on = false;

			protectCoroutine = null;
		}

		/// <summary>
		/// 吸铁石
		/// </summary>
		/// <returns>The execute.</returns>
		public IEnumerator MagnetiteExecute()
		{
//		var uiCtrl = GameManager.Instance.uiCtrl;

			GameManager.Instance.stageCtrl.MagnetiteOn();
//			GameManager.Instance.playerCtrl.MagnetiteEffectEmitter.Pause();

			int level = InfoManager.Instance.GetLevel("magnetite");
			float seconds = ConfigManager.Instance.GetPropConfigByName("magnetite").Times[level];
			if (prop_magnetite_on)
			{
			}
			else
			{
				prop_magnetite_on = true;
				GameManager.Instance.SendMsg(new QMsg((ushort) PropEvent.MagnetiteBegan));
			}

//		uiCtrl.ShowProp ("prop_magnetite",(int)seconds);
			yield return new WaitForSeconds(seconds - 1.8f);

//		uiCtrl.BlinkProp ("prop_magnetite");

			yield return new WaitForSeconds(1.8f);

			GameManager.Instance.SendMsg(new QMsg((ushort) PropEvent.MagnetiteEnded));
//			GameManager.Instance.playerCtrl.MagnetiteEffectEmitter.Pause();

			prop_magnetite_on = false;

			magnetiteCoroutine = null;
		}
	}
}