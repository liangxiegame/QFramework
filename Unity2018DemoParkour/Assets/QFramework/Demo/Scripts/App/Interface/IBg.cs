using UnityEngine;
using System.Collections;

/// <summary>
/// 背景
/// </summary>
public interface IBg{
	void Despawn();		// 回收掉背景
	void InitBg();		// 初始化背景
	void LerpPosY(float lerpValue); // 插值,前景高低的效果
}
