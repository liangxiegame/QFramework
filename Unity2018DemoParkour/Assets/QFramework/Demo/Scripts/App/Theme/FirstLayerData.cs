using System;

/// <summary>
/// 存储第一层数据的类
/// </summary>
public class FirstLayerData  {
	public int Length;
	public int[] Array = new int[10];
	public int EmptyCount = 0;
	public FirstLayerData(int emptyCount,int a0,int a1,int a2,int a3,int a4,int a5,int a6,int a7)
	{
		Length = 8;
		this.EmptyCount = emptyCount;
		Array [0] = a0;
		Array [1] = a1;
		Array [2] = a2;
		Array [3] = a3;
		Array [4] = a4;
		Array [5] = a5;
		Array [6] = a6;
		Array [7] = a7;
	}

	/// <summary>
	/// 第一层数据构造
	/// </summary>
	public FirstLayerData(int emptyCount,int a0,int a1,int a2,int a3,int a4,int a5,int a6,int a7,int a8,int a9)
	{
		Length = 10;
		this.EmptyCount = emptyCount;

		Array [0] = a0;
		Array [1] = a1;
		Array [2] = a2;
		Array [3] = a3;
		Array [4] = a4;
		Array [5] = a5;
		Array [6] = a6;
		Array [7] = a7;
		Array [8] = a8;
		Array [9] = a9;
	}
}
