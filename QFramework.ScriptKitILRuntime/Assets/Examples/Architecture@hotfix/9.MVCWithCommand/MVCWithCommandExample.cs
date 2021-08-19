using QFramework.ILRuntime;
using UniRx;

namespace QFramework.Example
{
	/// <summary>
	/// 将 Controller 层中，对 Model 层的修改部分代码，抽离到 Command 层中，来缓解 Controller 层职责过重的问题
	/// </summary>
	public partial class MVCWithCommandExample
	{
		void OnStart()
		{
			
			var model = MVCWithCommandConfig.GetModel<MVCWithCommandModel>();

			model.Count.BindWithInitialValue(count => Text.text = count.ToString()).AddTo(gameObject);

			Button.OnClickAsObservable().Subscribe(_ =>
			{
				MVCWithCommandConfig.SendCommand<IncreaseCountCommand>();
			});
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
		
		// 对 Model 的数据操作写在 Command 层
		public class IncreaseCountCommand : MVCWithCommandConfig.ICommand
		{
			public void Execute()
			{
				var model = MVCWithCommandConfig.GetModel<MVCWithCommandModel>();
				model.Count.Value++;
			}
		}
	}
	
	
	

	public class MVCWithCommandModel
	{
		public Property<int> Count = new Property<int>(0);
	}
	

	public class MVCWithCommandConfig
	{
		public interface ICommand
		{
			void Execute();
		}
		
		private static MVCWithCommandConfig mPrivateConfig = null;

		private static MVCWithCommandConfig mConfig
		{
			get
			{
				if (mPrivateConfig == null)
				{
					mPrivateConfig = new MVCWithCommandConfig();
					mPrivateConfig.Init();
				}

				return mPrivateConfig;
			}
		}
		
		private ILRuntimeIOCContainer mModelLayer = new ILRuntimeIOCContainer();

		public static T GetModel<T>() where T : class
		{
			return mConfig.mModelLayer.Resolve<T>();
		}
		
		
		private ILTypeEventSystem mEventSystem = new ILTypeEventSystem();

		void Init()
		{
			//  实现命令模式
			mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);

			// 注册模型
			mModelLayer.RegisterInstance(new MVCWithCommandModel());
		}

		public static void SendCommand<TCommand>() where TCommand : ICommand, new()
		{
			mConfig.mEventSystem.SendEvent<ICommand>(new TCommand());
		}

		public static void SendCommand<TCommand>(TCommand command) where TCommand : ICommand, new()
		{
			mConfig.mEventSystem.SendEvent<ICommand>(command);
		}
		
		void OnCommandExecute(ICommand command)
		{
			command.Execute();
		}
		
		public static void Dispose()
		{
			mConfig.mEventSystem.Clear();
			mConfig.mEventSystem = null;
		}
	}
}
