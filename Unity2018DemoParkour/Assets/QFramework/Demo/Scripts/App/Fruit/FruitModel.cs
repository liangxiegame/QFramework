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
using QF;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 处理水果的逻辑
	/// </summary>
	public class FruitModel : Singleton<FruitModel>
	{
		private FruitModel()
		{
		}

		public class FruitDataDefine
		{
			public  int    id;
			public  string name;
			private int    mEnergy;

			public int energy
			{
				set { mEnergy = value; }
				get { return PropModel.Instance.prop_fruit_x2_on ? mEnergy * 2 : mEnergy; }
			}

			public FruitDataDefine(int id, string name, int energy)
			{
				this.id = id;
				this.name = name;
				this.energy = energy;
			}
		}

		public const int basic_energy = 100;

		public Dictionary<string, FruitDataDefine> FruitDefines = new Dictionary<string, FruitDataDefine>()
		{
			{"mango", new FruitDataDefine(STAGE.FM, "mango", basic_energy)},
			{"banana", new FruitDataDefine(STAGE.FB, "banana", (int) (basic_energy * Mathf.Pow(1.05f, 1)))},
			{"coconut", new FruitDataDefine(STAGE.FC, "coconut", (int) (basic_energy * Mathf.Pow(1.05f, 2)))},
			{"quince", new FruitDataDefine(3, "prince", (int) (basic_energy * Mathf.Pow(1.05f, 3)))},
			{"pineapple", new FruitDataDefine(4, "pineapple", (int) (basic_energy * Mathf.Pow(1.05f, 4)))},
			{"pitaya", new FruitDataDefine(5, "pitaya", (int) (basic_energy * Mathf.Pow(1.05f, 5)))},
		};

		/// <summary>
		/// 处理水果消息
		/// </summary>
		/// <param name="name">Name.</param>
		public float HandleGetFruitEvent(string name)
		{
			float times = PropModel.Instance.prop_fruit_x2_on ? 2.0f : 1.0f;

			int energy = 0;
			switch (name)
			{
				case "fruit_banana":
					energy = FruitDefines["banana"].energy;
					break;

				case "fruit_coconut":
					energy = FruitDefines["coconut"].energy;
					break;

				case "fruit_mango":
					energy = FruitDefines["mango"].energy;
					break;

				case "fruit_pineapple":
					energy = FruitDefines["pineapple"].energy;
					break;

				case "fruit_pitaya":
					energy = FruitDefines["pitaya"].energy;
					break;

				case "fruit_quince":
					energy = FruitDefines["quince"].energy;
					break;
			}

			energy = (int) (energy * times);
//		energy = (int)(energy * QCSVMgr.Instance.GetValue("prop","name","fruit","time" + InfoManager.Instance.GetLevel("fruit")).FloatValue);
			GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.EnergyAdd, energy));

			return energy;
		}

		/// <summary>
		/// 随机生成水果数据
		/// </summary>
		/// <returns>The fruit data.</returns>
		public int GenFruitData()
		{
			int retValue = Random.Range(STAGE.FRUIT_BEGIN, STAGE.FRUIT_END);
			return retValue;
		}
	}
}