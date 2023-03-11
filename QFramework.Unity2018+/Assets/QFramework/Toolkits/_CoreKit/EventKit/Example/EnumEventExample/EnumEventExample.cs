/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
	public class EnumEventExample : MonoBehaviour
	{
		#region 事件定义

		public enum TestEvent
		{
			Start,
			TestOne,
			End,
		}

		public enum TestEventB
		{
			Start = TestEvent.End, // 为了保证每个消息 Id 唯一，需要头尾相接
			TestB,
			End,
		}

		#endregion 事件定义
		
		void Start()
		{
			EnumEventSystem.Global.Register(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
					Debug.Log(obj[0]);
					break;
			}
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				EnumEventSystem.Global.Send(TestEvent.TestOne, "Hello World!");
			}
		}

		private void OnDestroy()
		{
			EnumEventSystem.Global.UnRegister(TestEvent.TestOne, OnEvent);
		}
	}
}