/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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

using System;
using QF;
using QFramework;

public class ConfigManager : Singleton<ConfigManager>
{
	private ConfigManager()
	{
	}

	private LevelConfig[] mLevels;
	private PropConfig[]  mProps;

	public override void OnSingletonInit()
	{
		mLevels = new[]
		{
			new LevelConfig(1, 90, 5, 50, 100, 0, 3, 5),
			new LevelConfig(2, 85, 5, 50, 10, 0, 3, 5),
			new LevelConfig(3, 80, 5, 50, 10, 0, 3, 5),
			new LevelConfig(4, 75, 5, 50, 10, 0, 3, 5),
			new LevelConfig(5, 70, 15, 50, 10, 50, 3, 6),
			new LevelConfig(6, 65, 15, 50, 10, 50, 3, 6),
			new LevelConfig(7, 60, 15, 50, 10, 50, 3, 6),
			new LevelConfig(8, 55, 15, 50, 10, 50, 3, 6),
			new LevelConfig(9, 50, 20, 50, 10, 55, 4, 6),
			new LevelConfig(10, 45, 20, 50, 10, 55, 4, 6),
			new LevelConfig(11, 40, 20, 50, 10, 55, 4, 6),
			new LevelConfig(12, 30, 25, 50, 10, 60, 5, 6),
		};

		mProps = new[]
		{
			new PropConfig(0, "magnetite", 500, 1200, 3000, 5, 9, 16, 30, "在{0}秒内,能将附近的金币水果自动吸入"),
			new PropConfig(1, "big", 400, 1000, 2500, 5, 9, 16, 30, "在{0}秒内,人物处于变大状态,落地时震掉一切!"),
			new PropConfig(2, "protect", 300, 900, 2100, 5, 9, 16, 30, "在{0}秒内,大力神处于受保护,无惧任何障碍!"),
			new PropConfig(3, "auto", 1000, 2400, 5000, 5, 9, 16, 30, "在{0}秒内，无需控制大力神。"),
			new PropConfig(4, "goldX2", 500, 1200, 2900, 10, 15, 20, 25, "在{0}秒内,金币双倍。"),
			new PropConfig(5, "fruitX2", 600, 1400, 3000, 10, 15, 20, 25, "在{0}秒内,水果所补充的体力值双倍。"),
			new PropConfig(6, "fruit", 1000, 2500, 5500, 1, 1.33f, 1.66f, 1.99f, "水果所补充的体力值,永久提高{0}%。")
		};
	}

	public PropConfig GetPropConfigByName(string name)
	{
		return Array.Find(mProps, propConfig => propConfig.Name.Equals(name));
	}
}

public class LevelConfig
{
	public int Id;

	public int PropPercent;

	public int FruitPercent;

	public int GoldPercent;

	public int AirPercent;

	public int EnemyPercent;

	public int EmptyCountMin;

	public int EmptyCountMax;

	public LevelConfig(int id, int propPercent, int fruitPercent, int goldPercent, int airPercent, int enemyPercent,
		int emptyCountMin, int emptyCountMax)
	{
		Id = id;
		PropPercent = propPercent;
		FruitPercent = fruitPercent;
		GoldPercent = goldPercent;
		AirPercent = airPercent;
		EnemyPercent = enemyPercent;
		EmptyCountMin = emptyCountMin;
		EmptyCountMax = emptyCountMax;
	}
}


public class PropConfig
{
	public int Id;

	public string Name;

	public int[] Prices;

	public float[] Times;

	public string Description;

	public PropConfig(int id, string name, int price0, int price1, int price2, float time0, float time1, float time2,
		float time3, string description)
	{
		Id = id;
		Name = name;
		Prices = new[] {price0, price1, price2};
		Times = new[] {time0, time1, time2, time3};
		Description = description;
	}
}