using System;
using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class DelayExample : MonoBehaviour
	{
		
		void Start()
		{
			Debug.Log("当前时间为:" + DateTime.Now );

			// 对象模式
			var delay = DelayAction.Allocate(3, () =>
			{
				Debug.Log("延时了 3 秒");
				Debug.Log("当前时间为:" + DateTime.Now );
			});
			
			// 执行 delay 节点
			this.ExecuteNode(delay);
			
			
			// 简化版本（直接执行）
			this.Delay(5, () =>
			{
				Debug.Log("延时了 5 秒");
				Debug.Log("当前时间为:" + DateTime.Now );
			});
		}
	}
}