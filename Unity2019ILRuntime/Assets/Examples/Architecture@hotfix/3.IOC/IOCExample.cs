using UnityEngine;
using QFramework;
using QFramework.ILRuntime;

namespace QFramework.Example
{

	public class NetworkService
	{
		public void Connect()
		{
			Debug.Log("网络服务链接成功");			
		}
	}
	
	public partial class IOCExample
	{
		void OnStart()
		{
			// 创建一个 IOC 容器（可以理解为 Dictionary<Type,object>)
			var container = new ILRuntimeIOCContainer();
			
			// 注册一个类型 
			container.Register<NetworkService>();
			
			// 注册类型之后就可以根据类型获取对象(一般是自动创建)
			var networkService = container.Resolve<NetworkService>();
			
			// 获取对象之后就可以使用这个对象了
			networkService.Connect();
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}
