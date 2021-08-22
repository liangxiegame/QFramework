using UnityEngine;
using QFramework.ILRuntime;
using UniRx;

namespace QFramework.Example
{
	/// <summary>
	/// 事件父类
	/// </summary>
	public class SomeBaseEvent {}

	/// <summary>
	/// 事件继承事件
	/// </summary>
	public class ABCEvent : SomeBaseEvent
	{
		
	}
	
	
	/// <summary>
	/// ILTypeEvent
	/// </summary>
	public partial class ILTypeEventSystemInheritExample
	{
		void OnStart()
		{
			
			// 监听父类事件
			ILTypeEventSystem.Register<SomeBaseEvent>(OnSomeBaseEvent);

			Observable.EveryUpdate().Subscribe(_ =>
				{
					if (Input.GetMouseButtonDown(0))
					{
						// 发送 ABC 事件
						// 要指定父类事件，父类事件就能接收到
						ILTypeEventSystem.Send<SomeBaseEvent>(new ABCEvent());
					}
				})
				.AddTo(gameObject);

		}

		void OnSomeBaseEvent(SomeBaseEvent someBaseEvent)
		{
			Debug.Log(someBaseEvent.GetType());
		}

		void OnDestroy()
		{
			ILTypeEventSystem.UnRegister<SomeBaseEvent>(OnSomeBaseEvent);
		}
	}
}
