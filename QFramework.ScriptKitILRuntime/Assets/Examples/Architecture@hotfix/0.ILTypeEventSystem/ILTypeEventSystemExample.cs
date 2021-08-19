using UnityEngine;
using QFramework.ILRuntime;
using UniRx;

namespace QFramework.Example
{
	/// <summary>
	/// 定义一个事件
	/// </summary>
	public class SomeEvent
	{
		/// <summary>
		/// 定义一个值
		/// </summary>
		public int SomeValue { get; set; }
	}

	public partial class ILTypeEventSystemExample
	{
		void OnStart()
		{
			// 注册事件
			ILTypeEventSystem.Register<SomeEvent>(OnSomeEvent);


			Observable.EveryUpdate().Subscribe(_ =>
			{
				// 鼠标左键点击 
				if (Input.GetMouseButtonDown(0))
				{
					// 发送事件（不传参数)
					ILTypeEventSystem.Send<SomeEvent>();
					
					// 发送事件(传参数)
					ILTypeEventSystem.Send(new SomeEvent()
					{
						SomeValue = 123
					});

				}
			}).AddTo(gameObject);
		}

		void OnSomeEvent(SomeEvent someEvent)
		{
			Debug.Log(someEvent.SomeValue);
		}

		void OnDestroy()
		{
			// 注销事件
			ILTypeEventSystem.UnRegister<SomeEvent>(OnSomeEvent);
		}
	}
}
