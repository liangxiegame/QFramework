namespace QFramework.Example
{
	public class MainContainer : QFrameworkContainer, ISingleton
	{
		private MainContainer()
		{
		}

		public static IQFrameworkContainer Container
		{
			get { return SingletonProperty<MainContainer>.Instance; }
		}


		void ISingleton.OnSingletonInit()
		{
			// 注册网络服务模块
			RegisterInstance<INetworkExampleService>(new NetworkExampleService());
		}
	}
}