using System;
using System.Collections.Generic;

namespace QFramework
{
    public interface IArchitecture : ISingleton, IDisposable,
        ICanGetUtility,
        ICanGetModel,
        ICanRegisterSystem,
        ICanRegisterModel,
        ICanRegisterUtility,
        ICanSendEvent
    {
    }

    public abstract class Architecture<TConfig> : IArchitecture
        where TConfig : Architecture<TConfig>
    {
        protected Architecture()
        {
        }

        public static IArchitecture Interface
        {
            get { return SingletonProperty<TConfig>.Instance; }
        }

        public T GetSystem<T>() where T : class, ISystem
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterSystem<T>(T system) where T : class, ISystem
        {
            mContainer.RegisterInstance(system);
        }

        public void RegisterModel<T>(T model) where T : class, IModel
        {
            mContainer.RegisterInstance(model);
        }

        public T GetUtility<T>() where T : class, IUtility
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterUtility<T>(T utility) where T : class, IUtility
        {
            mContainer.RegisterInstance<T>(utility);
        }

        private readonly IQFrameworkContainer mContainer = new QFrameworkContainer();

        protected readonly ITypeEventSystem mEventSystem = new TypeEventSystem();

        public T GetModel<T>() where T : class, IModel
        {
            return mContainer.Resolve<T>();
        }
        
        public void SendEvent<T>() where T : new()
        {
            mEventSystem.SendEvent<T>();
        }
        
        public void SendEvent<T>(T @event)
        {
            mEventSystem.SendEvent<T>(@event);
        }


        public void OnSingletonInit()
        {
            // 注册命令模式
            RegisterCommand();
            OnUtilityConfig(mContainer);
            OnModelConfig(mContainer);
            OnSystemConfig(mContainer);
            OnLaunch();
        }

        protected virtual void RegisterCommand()
        {
            mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
        }

        protected virtual void OnCommandExecute(ICommand command)
        {
            command.Execute();
        }

        protected virtual void UnRegisterCommand()
        {
            mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
        }

        public virtual void SendCommand<T>() where T : ICommand, new()
        {
            mEventSystem.SendEvent<ICommand>(new T());
        }

        public virtual void SendCommand(ICommand command)
        {
            mEventSystem.SendEvent<ICommand>(command);
        }


        protected abstract void OnSystemConfig(IQFrameworkContainer systemLayer);
        protected abstract void OnModelConfig(IQFrameworkContainer modelLayer);
        protected abstract void OnUtilityConfig(IQFrameworkContainer utilityLayer);

        protected abstract void OnLaunch();

        public void Dispose()
        {
            OnDispose();
            if (mContainer != null) mContainer.Dispose();
            if (mEventSystem != null) mEventSystem.Dispose();
        }

        protected virtual void OnDispose()
        {
        }




        public IDisposable RegisterEvent<T>(Action<T> onEvent)
        {
            return mEventSystem.RegisterEvent<T>(onEvent);
        }
    }

    public abstract class Architecture<TCommand, TConfig> :
        Architecture<TConfig>
        where TCommand : class, ICommand
        where TConfig : Architecture<TCommand, TConfig>
    {
        public override void SendCommand<T>()
        {
            mEventSystem.SendEvent<TCommand>(new T() as TCommand);
        }

        public override void SendCommand(ICommand command)
        {
            mEventSystem.SendEvent<ICommand>(command);
        }
    }

    public abstract class AbstractPool<T> where T : AbstractPool<T>, new()
    {
        private static Stack<T> mPool = new Stack<T>(10);

        protected bool mInPool = false;

        public static T Allocate()
        {
            var node = mPool.Count == 0 ? new T() : mPool.Pop();
            node.mInPool = false;
            return node;
        }

        public void Recycle2Cache()
        {
            OnRecycle();
            mInPool = true;
            mPool.Push(this as T);
        }

        protected abstract void OnRecycle();
    }
}