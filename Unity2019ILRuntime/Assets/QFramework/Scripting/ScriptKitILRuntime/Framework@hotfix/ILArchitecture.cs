using System;
using QFramework.ILRuntime;

namespace QFramework
{
    public interface ILArchitecture  : 
        ILCanGetUtility,
        ILCanGetSystem,
        ILCanGetModel,
        ILCanSendCommand,
        ILCanSendEvent,
        ILCanRegisterUtility,
        ILCanRegisterSystem,
        ILCanRegisterModel,
        ILCanRegisterEvent,
        ILHasInit
    {
    }

    public abstract class ILArchitecture<TArchitecture> : ILArchitecture
        where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public T GetSystem<T>() where T : class, ILSystem
        {
            return mContainer.Resolve<T>();
        }

        public  void RegisterSystem<T>(T system) where T : class, ILSystem
        {
            mContainer.RegisterInstance<T>(system);
        }

        public T GetModel<T>() where T : class, ILModel
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterModel<T>(T model) where T : class, ILModel
        {
            mContainer.RegisterInstance<T>(model);
        }

        public T GetUtility<T>() where T : class, ILUtility
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterUtility<T>(T utility) where T : class, ILUtility
        {
            mContainer.RegisterInstance<T>(utility);
        }

        public void SendCommand<T>() where T : ILCommand, new()
        {
            
            mEventSystem.SendEvent<ILCommand>(new T());
        }

        public void SendCommand<T>(T t) where T : ILCommand
        {
            mEventSystem.SendEvent<ILCommand>(t);

        }

        
        public void SendEvent<T>() where T : new()
        {
            mEventSystem.SendEvent<T>();
        }

        public void SendEvent<T>(T @event) where T : class
        {
            mEventSystem.SendEvent<T>(@event);
        }

        public ILCanDispose RegisterEvent<T>(Action<T> onEvent)
        {
            return mEventSystem.RegisterEvent<T>(onEvent);
        }

        public static ILTypeEventSystem EventSystem
        {
            get { return mConfig.mEventSystem; }
        }
        
        private readonly ILRuntimeIOCContainer mContainer = new ILRuntimeIOCContainer();

        protected readonly ILTypeEventSystem mEventSystem = new ILTypeEventSystem();

        private static ILArchitecture<TArchitecture> @private = null;

        protected static ILArchitecture<TArchitecture> mConfig => ILSingleton<TArchitecture>.Instance;



        protected virtual void OnCommandExecute(ILCommand command)
        {
            command.Execute();
        }
        
        public void Dispose()
        {
            mEventSystem.UnRegisterEvent<ILCommand>(OnCommandExecute);
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        protected abstract void OnSystemConfig(ILRuntimeIOCContainer systemLayer);
        protected abstract void OnModelConfig(ILRuntimeIOCContainer modelLayer);
        protected abstract void OnUtilityConfig(ILRuntimeIOCContainer utilityLayer);

        protected abstract void OnLaunch();
        

        public void OnInit()
        {
            // 注册命令模式
            mEventSystem.RegisterEvent<ILCommand>(OnCommandExecute);
            OnUtilityConfig(mContainer);
            OnModelConfig(mContainer);
            OnSystemConfig(mContainer);
            OnLaunch();
        }
    }
}