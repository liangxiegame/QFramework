using UnityEngine;
using System.Collections;

public class Theme4DataDefine {

	/// <summary>
	/// 0是空,1是向左,2是中间,3是向右,4是air
	/// 数据应该存xml或者
	/// </summary>
	public static FirstLayerData[] FirstLayerZeroEmpty = {
		// 0个空心
		new FirstLayerData(EMPTY.ZERO, 2, 2, 2, 2, 2, 2, 2, 2),
	};

	public static FirstLayerData[] FirstLayerOneEmpty = {
		// 1个空心
		new FirstLayerData(EMPTY.ONE, 3, 0, 1, 2, 2, 2, 2, 2),
		new FirstLayerData(EMPTY.ONE, 2, 3, 0, 1, 2, 2, 2, 2),
		new FirstLayerData(EMPTY.ONE, 2, 2, 3, 0, 1, 2, 2, 2),
		new FirstLayerData(EMPTY.ONE, 2, 2, 2, 3, 0, 1, 2, 2),
		new FirstLayerData(EMPTY.ONE, 2, 2, 2, 2, 3, 0, 1, 2),
		new FirstLayerData(EMPTY.ONE, 2, 2, 2, 2, 2, 3, 0, 1),
	};

	public static FirstLayerData[] FirstLayerTwoEmpty = {
		// 2个空心
		new FirstLayerData(EMPTY.TWO, 3, 0, 0, 1, 2, 2, 2, 2),
		new FirstLayerData(EMPTY.TWO, 2, 3, 0, 0, 1, 2, 2, 2),
		new FirstLayerData(EMPTY.TWO, 2, 2, 3, 0, 0, 1, 2, 2),
		new FirstLayerData(EMPTY.TWO, 2, 2, 2, 3, 0, 0, 1, 2),
		new FirstLayerData(EMPTY.TWO, 2, 2, 2, 2, 3, 0, 0, 1),

		new FirstLayerData(EMPTY.TWO, 3, 0, 1, 3, 0, 1, 2, 2),
		new FirstLayerData(EMPTY.TWO, 3, 0, 1, 2, 3, 0, 1, 2),
		new FirstLayerData(EMPTY.TWO, 3, 0, 1, 2, 2, 3, 0, 1),

		new FirstLayerData(EMPTY.TWO, 2, 3, 0, 1, 3, 0, 1, 2),
		new FirstLayerData(EMPTY.TWO, 2, 3, 0, 1, 2, 3, 0, 1),

		new FirstLayerData(EMPTY.TWO, 2, 2, 3, 0, 1, 3, 0, 1),
	};

	public static FirstLayerData[] FirstLayerThreeEmpty = {
		//3个空心
		new FirstLayerData(EMPTY.THREE, 3, 0, -1, 0, 1, 2, 2, 2 ),
		new FirstLayerData(EMPTY.THREE, 2, 3, 0, -1, 0, 1, 2, 2 ),
		new FirstLayerData(EMPTY.THREE, 2, 2, 3, 0, -1, 0, 1, 2 ),
		new FirstLayerData(EMPTY.THREE, 2, 2, 2, 3, 0, -1, 0, 1 ),


		new FirstLayerData(EMPTY.THREE, 3, 0, 0, 1, 3, 0, 1, 2),
		new FirstLayerData(EMPTY.THREE, 3, 0, 0, 1, 2, 3, 0, 1),

		new FirstLayerData(EMPTY.THREE, 3, 0, 1, 3, 0, 0, 1, 2),
		new FirstLayerData(EMPTY.THREE, 3, 0, 1, 2, 3, 0, 0, 1),

		new FirstLayerData(EMPTY.THREE, 2, 3, 0, 0, 1, 3, 0, 1),
		new FirstLayerData(EMPTY.THREE, 2, 3, 0, 1, 3, 0, 0, 1),
	};

	public static FirstLayerData[] FirstLayerFourEmpty = {
		// 四个空心
		new FirstLayerData(EMPTY.FOUR, 3, 0, -1, -1, 0, 1, 2, 2),
		new FirstLayerData(EMPTY.FOUR, 3, 0, -1, 0, 1, 3, 0, 1),

		new FirstLayerData(EMPTY.FOUR, 3, 0, 0, 1, 3, 0, 0, 1),

		new FirstLayerData(EMPTY.FOUR, 3, 0, 0, 1, 3, 0, 1, 2),

		new FirstLayerData(EMPTY.FOUR, 3, 0, 1, 3, 0, -1, 0, 1),
		new FirstLayerData(EMPTY.FOUR, 3, 0, 1, 3, 0, 0, 1, 2),

		new FirstLayerData(EMPTY.FOUR, 2, 3, 0, -1, -1, 0, 1, 2),

		new FirstLayerData(EMPTY.FOUR, 2, 2, 3, 0, -1, -1, 0, 1),
	};

	public static FirstLayerData[] FirstLayerFiveEmpty = {
		// 五个空心
		new FirstLayerData(EMPTY.FIVE, 3, 0, -1, -1, -1, 0, 1, 2),
		new FirstLayerData(EMPTY.FIVE, 2, 3, 0, -1, -1, -1, 0, 1),
	};

	public static FirstLayerData[] FirstLayerSixEmpty = {
		// 六个空心
		new FirstLayerData(EMPTY.SIX, 3, 0, -1, -1, -1, -1, 0, 1),
	};


	public static FirstLayerData EmptyCount(int count)
	{
		switch (count) {
		case 0:
			return FirstLayerZeroEmpty [0];
		case 1:
			return FirstLayerOneEmpty[Random.Range(0,FirstLayerOneEmpty.Length)];
		case 2:
			return FirstLayerTwoEmpty[Random.Range(0,FirstLayerTwoEmpty.Length)];
		case 3:
			return FirstLayerThreeEmpty [Random.Range (0, FirstLayerThreeEmpty.Length)];
		case 4:
			return FirstLayerFourEmpty [Random.Range (0, FirstLayerFourEmpty.Length)];
		case 5:
			return FirstLayerFiveEmpty [Random.Range (0, FirstLayerFiveEmpty.Length)];
		case 6:
			return FirstLayerSixEmpty [Random.Range (0, FirstLayerSixEmpty.Length)];
		}
		return null;
	}
}
