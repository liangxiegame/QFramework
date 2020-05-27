// using UnityEngine;
//
// namespace QFramework
// {
// 	[MonoSingletonPath("QFramework")]
// 	public class Framework : MonoBehaviour, ISingleton, IFramework
// 	{
// 		public IQFrameworkContainer RootContainer { get; private set; }
// 		
// 		private static IFramework mIntance
// 		{
// 			get { return MonoSingletonProperty<Framework>.Instance; }
// 		}
// 		
// 		// public static T GetModule<T>() where T : class, IModule
// 		// {
// 		// 	return mIntance.RootContainer.Resolve<T>();
// 		// }
// 		//
// 		// public static void RegisterModule<T>(K k) where T : class, IModule
// 		// {
// 		// 	return mIntance.RootContainer.RegisterInstance<T>(new K());
// 		// }
// 		
// 		public void OnSingletonInit()
// 		{
// 			RootContainer = new QFrameworkContainer();
// 		}
//
// 		void Call()
// 		{
// 		}
//
// 		[RuntimeInitializeOnLoadMethod]
// 		static void InitOnLoad()
// 		{
// 			var framework = mIntance;
// 			(framework as Framework).Call();
// 		}
// 	}
// }