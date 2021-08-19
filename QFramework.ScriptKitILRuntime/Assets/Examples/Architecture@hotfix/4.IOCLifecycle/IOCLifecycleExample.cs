using UnityEngine;
using QFramework;
using QFramework.ILRuntime;

namespace QFramework.Example
{
	public class SomeClassA {}
	public class SomeClassB {}
	
	public partial class IOCLifecycleExample
	{
		void OnStart()
		{
			// Code Here
			
			var container = new ILRuntimeIOCContainer();
			
			// 短暂注册 (每次获取，都会返回一个新的对象)
			container.Register<SomeClassA>();
			
			// 注册为单例（每次获取都返回同一个对象)
			container.RegisterInstance(new SomeClassB());
			
			
			
			// hashCode 一致，说明对象一致，不一致，说明不是同一个对象
			Debug.Log(container.Resolve<SomeClassA>().GetHashCode());
			Debug.Log(container.Resolve<SomeClassA>().GetHashCode());
			Debug.Log(container.Resolve<SomeClassA>().GetHashCode());
			
			Debug.Log(container.Resolve<SomeClassB>().GetHashCode());
			Debug.Log(container.Resolve<SomeClassB>().GetHashCode());
			Debug.Log(container.Resolve<SomeClassB>().GetHashCode());
			
			
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}
