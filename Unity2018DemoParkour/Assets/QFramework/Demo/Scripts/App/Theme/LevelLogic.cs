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
	/// 等级查询
	/// </summary>
	public class LevelLogic
	{

		public class LevelData
		{
			public int propPercent;
			public int fruitPercent;
			public int goldPercent;
			public int airPercent;
			public int enemyPercent;
			public int emptyCountMin;
			public int emptyCountMax;

			public LevelData(int propPercent, int fruitPercent, int goldPercent, int airPercent, int enemyPercent,
				int emptyCountMin, int emptyCountMax)
			{
				this.propPercent = propPercent;
				this.fruitPercent = fruitPercent;
				this.goldPercent = goldPercent;
				this.airPercent = airPercent;
				this.enemyPercent = enemyPercent;
				this.emptyCountMin = emptyCountMin;
				this.emptyCountMax = emptyCountMax;
			}
		}

		/// <summary>
		/// 所有和关卡有关的数据都卸载这里
		/// </summary>
		public static LevelData[] data =
		{
			new LevelData(90, 5, 50, 10, 100, 3, 5), // level 0
			new LevelData(85, 5, 50, 10, 0, 3, 5), // level 1
			new LevelData(80, 5, 50, 10, 0, 3, 5), // level 2
			new LevelData(75, 5, 50, 10, 0, 3, 5), // level 3

			new LevelData(70, 15, 50, 10, 50, 3, 6), // level 4
			new LevelData(65, 15, 50, 10, 50, 3, 6), // level 5
			new LevelData(60, 15, 50, 10, 50, 3, 6), // level 6
			new LevelData(55, 15, 50, 10, 50, 3, 6), // level 7

			new LevelData(50, 20, 50, 10, 55, 4, 6), // level 8
			new LevelData(45, 20, 50, 10, 55, 4, 6), // level 9
			new LevelData(40, 20, 50, 10, 55, 4, 6), // level 10
			new LevelData(35, 20, 50, 10, 55, 4, 6), // level 11

			new LevelData(30, 25, 50, 10, 60, 5, 6), // level 12
			new LevelData(25, 25, 50, 10, 60, 5, 6), // level 13
			new LevelData(20, 25, 50, 10, 60, 5, 6), // level 14
			new LevelData(15, 25, 50, 10, 60, 5, 6), // level 15
		};

		public static LevelData DataForLevel()
		{
			return data[PlatformRunner.Interface.GetModel<IGameModel>().CurLevel.Value];
		}
	}
}