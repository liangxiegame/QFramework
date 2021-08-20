using UnityEngine;
using System.Collections;


/// <summary>
/// 数据格式
/// </summary>
public class StageData
{
	/// <summary>
	/// 定义个数
	/// </summary>
	public int FirstNumCount = 10;
	public int SecondNumCount = 10;
	public int ForeCount = 10;
	public int ThirdNumCount = 5;
	public int FourthNumCount = 5;

	/// <summary>
	/// 定义数字
	/// </summary>
	public int[] FirstNums = new int[10];
	public int[] SecondNums = new int[10];
	public int[] ForeNums = new int[10];
	public int[] ThirdNums = new int[5];
	public int[] FourthNums = new int[5];

	public int Theme = 0;

	public IStageDataGenerator Generator = null;

	public void setGenerator(IStageDataGenerator stageGenerator)
	{
		Generator = stageGenerator;
	}

	/// <summary>
	/// 设置主题
	/// </summary>
	public void setTheme(int theme)
	{
		switch (theme) {
		case 1:
			FirstNumCount = 10;
			SecondNumCount = 10;
			ForeCount = 10;
			ThirdNumCount = 5;
			FourthNumCount = 5;
			break;
		case 2:
			FirstNumCount = 8;
			SecondNumCount = 8;
			ForeCount = 8;
			ThirdNumCount = 5;
			FourthNumCount = 5;
			break;	
		case 3:
			FirstNumCount = 8;
			SecondNumCount = 8;
			ForeCount = 8;
			ThirdNumCount = 0;
			FourthNumCount = 5;
			break;
		case 4:
			FirstNumCount = 8;
			SecondNumCount = 8;
			ForeCount = 8;
			ThirdNumCount = 5;
			FourthNumCount = 5;
			break;
		}
		Theme = theme;
	}
}