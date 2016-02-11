using UnityEngine;
using System.Collections;

/// <summary>
/// MVC Model 模型接口
/// </summary>
public interface IModel  {
	void InitModel();	// 初始化模型,游戏运行后只执行一次
	void ResetModel();	// 重置模型逻辑 
}
