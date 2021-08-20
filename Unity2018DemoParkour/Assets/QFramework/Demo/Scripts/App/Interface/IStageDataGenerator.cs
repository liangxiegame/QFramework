using UnityEngine;
using System.Collections;

/// <summary>
/// 关卡数据生成
/// </summary>
public interface IStageDataGenerator {
	Vector3[] FirstLayerPos(); 		// 第一层位置
	Vector3[] ThirdLayerPos();		// 第三层位置

	int Theme();					

	StageData GetStageData();
	string    GetStageName();
}
