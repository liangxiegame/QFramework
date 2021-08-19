using UnityEngine;
using QFramework;
using QFramework.ILRuntime;

namespace QFramework.Example
{
	/// <summary>
	/// 定义一个接口
	/// </summary>
	public interface INetworkService
	{
		/// <summary>
		/// 连接网络
		/// </summary>
		void Connect();
	}

	/// <summary>
	/// 开发服务器服务
	/// </summary>
	public class DevNetworkService : INetworkService
	{
		public void Connect()
		{
			Debug.Log("链接到开发服务器");
		}
	}

	/// <summary>
	/// 正式服务器服务
	/// </summary>
	public class RealNetworkService : INetworkService
	{
		public void Connect()
		{
			Debug.Log("链接到正式服务器");
		}
	}

	/// <summary>
	/// DIP: 意思是依赖倒置原则
	/// </summary>
	public partial class IOCWithDIPExample
	{
		void OnStart()
		{
			var container = new ILRuntimeIOCContainer();
			
			// 注册开发服务器
			container.RegisterInstance<INetworkService>(new DevNetworkService());
			
			// 链接服务器
			container.Resolve<INetworkService>().Connect();
			
			// 切换为正式服务器
			container.RegisterInstance<INetworkService>(new RealNetworkService());
			
			// 链接服务器
			container.Resolve<INetworkService>().Connect();
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}
