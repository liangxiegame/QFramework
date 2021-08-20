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
using QF;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 定义关卡数据和种类
	/// 根据等级和概率生成
	/// </summary>
	public class Stage2DataGenerator : Singleton<Stage2DataGenerator>, IStageDataGenerator
	{

		public Queue stageDatas  = new Queue();
		public Stack unusedDatas = new Stack();

		private Stage2DataGenerator()
		{
		}

		public  LevelLogic.LevelData levelData;
		private FirstLayerData       firstLayerData;

		private Coroutine CoGenData;

		public void InitGenerator()
		{
			stageDatas.Clear();

			if (CoGenData != null)
			{
				GameManager.Instance.StopCoroutine(CoGenData);
				CoGenData = null;
			}

			CoGenData = GameManager.Instance.StartCoroutine(GenData());
		}

		public Vector3[] BottomBlockPos = new Vector3[8]; // 第一层的位置数据 可以是 块,金币,道具,水果

		// 第二层的位置为底部块的位置的y坐标 + 1.5 可以是金币,或者敌人。
		public Vector3[] AirBlockPos = new Vector3[5]; // 第三层的位置数据可以是块,金币,水果,道具
		// 第四层的位置为第三层块的位置的y坐标 + 1.5 可以是金币,或者是水果,或者是道具。

		public Vector3[] FirstLayerPos()
		{
			return BottomBlockPos;
		}

		public Vector3[] ThirdLayerPos()
		{
			return AirBlockPos;
		}

		public int Theme()
		{
			return 2;
		}

		public void ParseStagePattern(Transform stagePattern)
		{
			for (int i = 0; i < 8; i++)
			{
				BottomBlockPos[i] = stagePattern.Find("block_pos" + (i + 1)).localPosition;
			}

			for (int i = 0; i < 5; i++)
			{
				AirBlockPos[i] = stagePattern.Find("block_air_pos" + (i + 1)).localPosition;
			}

			GameObject.Destroy(stagePattern.gameObject);
		}

		int GenFirstLayerData(int index)
		{
			int retNumber = 0;
			if (firstLayerData.Array[index] == -1)
			{
				if (ProbilityHelper.PercentProbability(levelData.goldPercent))
				{
					retNumber = STAGE.G6;
				}
				else
				{
					retNumber = STAGE.EM;
				}
			}
			else
			{
				retNumber = firstLayerData.Array[index];
			}

			return retNumber;
		}

		int GenComponentData(int index)
		{
			int retNumber = STAGE.BA;

			if (firstLayerData.Array[index] != 0
			    && firstLayerData.Array[index] != -1)
			{

				switch (firstLayerData.Array[index])
				{
					case STAGE.BL:
						if (ProbilityHelper.PercentProbability(50))
						{
							retNumber = ProbilityHelper.RandomValueFrom(STAGE.C2, STAGE.C3, STAGE.C10, STAGE.C14, STAGE.C16);
						}

						break;
					case STAGE.BM:
						if (ProbilityHelper.PercentProbability(33))
						{
							retNumber = ProbilityHelper.RandomValueFrom(STAGE.C1, STAGE.C5, STAGE.C7, STAGE.C8, STAGE.C9, STAGE.C12);
						}

						break;
					case STAGE.BR:
						if (ProbilityHelper.PercentProbability(50))
						{
							retNumber = ProbilityHelper.RandomValueFrom(STAGE.C4, STAGE.C6, STAGE.C11, STAGE.C13, STAGE.C15);
						}

						break;
				}
			}

			if (ProbilityHelper.PercentProbability(10))
			{
//			retNumber = QMath.RandomWithParams (STAGE.C17, STAGE.C18, STAGE.C19, STAGE.C20);
				retNumber = ProbilityHelper.RandomValueFrom(STAGE.C19, STAGE.C20);
			}

			return retNumber;
		}

		int GenSecondLayerData(int index)
		{
			int secondNum = STAGE.EM;

			// 第二层数据
			if (firstLayerData.Array[index] != 0 && firstLayerData.Array[index] != -1)
			{
				int chanceNum = Random.Range(0, 3); // 整型是[),浮点型是[]

				switch (chanceNum)
				{
					case 0:
						secondNum = STAGE.EM;
						break;
					case 1:
						if (ProbilityHelper.PercentProbability(levelData.goldPercent)
						    && firstLayerData.Array[index] != STAGE.BL
						    && firstLayerData.Array[index] != STAGE.BR)
						{
							secondNum = ProbilityHelper.RandomValueFrom(STAGE.G3, STAGE.G6);
						}

						break;
					case 2:
						if (ProbilityHelper.PercentProbability(levelData.enemyPercent))
						{
							secondNum = Random.Range(STAGE.ENEMY_BEGIN, STAGE.ENEMY_END);
						}

						break;
					default:
						break;
				}
			}

			return secondNum;
		}

		// 第三层数据生成
		int GenThirdLayerData(int previousData, int index)
		{
			int retNumber = STAGE.EM;

			if (ProbilityHelper.PercentProbability(levelData.airPercent))
			{
				if (previousData != STAGE.BA)
				{
					retNumber = STAGE.BA;
				}
			}

			return retNumber;
		}

		// 第四层数据生成
		int GenFourthLayerData(int thirdNum, int index)
		{
			int retNumber = 0;

			if (thirdNum != 0)
			{
				retNumber = ProbilityHelper.RandomValueFrom(PropModel.Instance.GenPropData(), STAGE.GC, STAGE.EM);
			}
			else if (ProbilityHelper.PercentProbability(levelData.fruitPercent))
			{
				retNumber = FruitModel.Instance.GenFruitData();
			}

			return retNumber;
		}

		/// <summary>
		/// to do list 生成数据要变成异步执行
		/// </summary>
		/// <returns>The data.</returns>
		public IEnumerator GenData()
		{
			levelData = LevelLogic.DataForLevel();
			firstLayerData = Theme2DataDefine.EmptyCount(Random.Range(levelData.emptyCountMin, levelData.emptyCountMax + 1));

			Debug.LogWarning("stageDatas:gen");

			// 这个new以后要kill掉
			StageData retStageData = new StageData();
			retStageData.setGenerator(this);
			retStageData.setTheme(2);

			// 底层数据
			for (int i = 0; i < 8; i++)
			{
				// 第一层数据 
				retStageData.FirstNums[i] = GenFirstLayerData(i);
				yield return new WaitForEndOfFrame();

				retStageData.SecondNums[i] = GenSecondLayerData(i);
				yield return new WaitForEndOfFrame();

				retStageData.ForeNums[i] = GenComponentData(i);
				yield return new WaitForEndOfFrame();
			}

			// 第三层数据
			if (firstLayerData.EmptyCount >= EMPTY.FIVE)
			{

				int baIndex = ProbilityHelper.RandomValueFrom(1, 2);

				for (int i = 0; i < 5; i++)
				{
					if (baIndex == i)
					{
						retStageData.ThirdNums[i] = STAGE.BA;
					}
					else
					{
						retStageData.ThirdNums[i] = STAGE.EM;
					}

					yield return new WaitForEndOfFrame();
				}

			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					retStageData.ThirdNums[i] = GenThirdLayerData(i > 0 ? retStageData.ThirdNums[i] : 0, i);
					yield return new WaitForEndOfFrame();
				}
			}

			// 第四层数据
			for (int i = 0; i < 5; i++)
			{
				retStageData.FourthNums[i] = GenFourthLayerData(retStageData.ThirdNums[i], i);
				yield return new WaitForEndOfFrame();
			}

			yield return GameManager.Instance.StartCoroutine(GenData());
		}

		/// <summary>
		/// 生成关卡数据返回给StageCtrl 
		/// </summary>
		public StageData GetStageData()
		{
			if (stageDatas.Count != 0)
			{
				Debug.LogWarning("stageDatas count:" + stageDatas.Count);
				var retData = stageDatas.Dequeue() as StageData;
				unusedDatas.Push(retData);
				return retData;
			}

			Debug.LogWarning("stageDatas:new");
			levelData = LevelLogic.DataForLevel();
			firstLayerData = Theme2DataDefine.EmptyCount(Random.Range(levelData.emptyCountMin, levelData.emptyCountMax + 1));
			StageData retStageData = null;
			if (unusedDatas.Count == 0)
			{
				retStageData = new StageData();
				retStageData.setGenerator(this);
				retStageData.setTheme(2);
			}
			else
			{
				retStageData = unusedDatas.Pop() as StageData;
			}

			// 第一层数据
			for (int i = 0; i < 8; i++)
			{
				retStageData.FirstNums[i] = GenFirstLayerData(i);
				retStageData.SecondNums[i] = GenSecondLayerData(i);
				retStageData.ForeNums[i] = GenComponentData(i);
			}

			// 第三层数据
			if (firstLayerData.EmptyCount >= EMPTY.FIVE)
			{

				int baIndex = ProbilityHelper.RandomValueFrom(1, 2);

				for (int i = 0; i < 5; i++)
				{
					if (baIndex == i)
					{
						retStageData.ThirdNums[i] = STAGE.BA;
					}
					else
					{
						retStageData.ThirdNums[i] = STAGE.EM;
					}
				}

			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					retStageData.ThirdNums[i] = GenThirdLayerData(i > 0 ? retStageData.ThirdNums[i] : 0, i);
				}
			}

			// 第四层数据
			for (int i = 0; i < 5; i++)
			{
				retStageData.FourthNums[i] = GenFourthLayerData(retStageData.ThirdNums[i], i);
			}

			unusedDatas.Push(retStageData);

			return retStageData;
		}

		// 每隔一个用一个

		/// <summary>
		/// 关卡名字
		/// </summary>
		int StageCount = 0;

		public string GetStageName()
		{
			StageCount++;

			string retString = "CustomStage1";

			switch (StageCount)
			{
				case 1:
					break;
				case 2:
					retString = ProbilityHelper.RandomValueFrom(
						"CustomStage1",
						"CustomStage2",
						"CustomStage3",
						"CustomStage4",
						"CustomStage5",
						"CustomStage6",
						"CustomStage7",
						"CustomStage8",
						"CustomStage9",
						"CustomStage10",
						"CustomStage11",
						"CustomStage12",
						"CustomStage13",
						"CustomStage14",
						"CustomStage15");
					StageCount = 0;
					break;
			}

			Debug.LogWarning("Theme 2:" + retString);
			// 测试
			//		retString = "CustomStage5";
			return retString;
		}
	}
}