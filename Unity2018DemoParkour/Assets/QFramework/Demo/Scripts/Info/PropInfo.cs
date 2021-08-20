using UnityEngine;
using System.Collections;


/// <summary>
/// 道具信息
/// </summary>
public class PropInfo {

	/// <summary>
	/// 名字
	/// </summary>
	public string name;


	/// <summary>
	/// 等级
	/// </summary>
	public int level;


	public PropInfo(string name,int level)
	{
		this.name = name;
		this.level = level;
	}
}
