using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public abstract class AbstractArchitectureConfig<TCommand, TConfig>
        where TCommand : ICommand
        where TConfig : AbstractArchitectureConfig<TCommand, TConfig>, new()
    {
        // 提供给大家主动调用一下
        public static void Init()
        {
            Debug.Log("@@@@ " + mConfig.GetType().Name + " 初始化");
        }

        public static T GetSystem<T>() where T : class, ISystem
        {
            return mConfig.mSystemLayer.Resolve<T>();
        }

        public static void RegisterSystem<T>(T system) where T : class, ISystem
        {
            mConfig.mSystemLayer.RegisterInstance<T>(system);
        }

        public static T GetModel<T>() where T : class, IModel
        {
            return mConfig.mModelLayer.Resolve<T>();
        }

        public static void RegisterModel<T>(T model) where T : class, IModel
        {
            mConfig.mModelLayer.RegisterInstance<T>(model);
        }

        public static T GetUtility<T>() where T : class, IUtility
        {
            return mConfig.mUtilityLayer.Resolve<T>();
        }

        public static void RegisterUtility<T>(T utility) where T : class, IUtility
        {
            mConfig.mUtilityLayer.RegisterInstance<T>(utility);
        }

        public static void SendCommand<T>() where T : TCommand, new()
        {
            mConfig.mEventSystem.SendEvent<TCommand>(new T());
        }

        public static void SendCommand(TCommand command)
        {
            mConfig.mEventSystem.SendEvent<TCommand>(command);
        }

        public static void SendEvent<T>() where T : new()
        {
            mConfig.mEventSystem.SendEvent<T>();
        }

        public static void SendEvent<T>(T @event)
        {
            mConfig.mEventSystem.SendEvent<T>(@event);
        }

        public static void RegisterEvent<T>(Action<T> onEvent)
        {
            mConfig.mEventSystem.RegisterEvent<T>(onEvent);
        }

        private IQFrameworkContainer mSystemLayer = new QFrameworkContainer();
        private IQFrameworkContainer mModelLayer = new QFrameworkContainer();
        private IQFrameworkContainer mUtilityLayer = new QFrameworkContainer();

        protected ITypeEventSystem mEventSystem = new TypeEventSystem();

        private static AbstractArchitectureConfig<TCommand, TConfig> mPrivateConfig = null;

        protected static AbstractArchitectureConfig<TCommand, TConfig> mConfig
        {
            get
            {
                if (mPrivateConfig == null)
                {
                    mPrivateConfig = new TConfig();
                    mPrivateConfig.Config();
                }

                return mPrivateConfig;
            }
        }

        protected void Config()
        {
            // 注册命令模式
            mEventSystem.RegisterEvent<TCommand>(OnCommandExecute);
            OnUtilityConfig(mUtilityLayer);
            OnModelConfig(mModelLayer);
            OnSystemConfig(mSystemLayer);
        }

        protected virtual void OnCommandExecute(TCommand command)
        {
            command.Execute();
        }

        public void Dispose()
        {
            mEventSystem.UnRegisterEvent<TCommand>(OnCommandExecute);
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        protected abstract void OnSystemConfig(IQFrameworkContainer systemLayer);
        protected abstract void OnModelConfig(IQFrameworkContainer modelLayer);
        protected abstract void OnUtilityConfig(IQFrameworkContainer utilityLayer);


        public abstract class AbstractEventSystemNode<TEventSystemNode> : AbstractPool<TEventSystemNode>
            where TEventSystemNode : AbstractEventSystemNode<TEventSystemNode>, new()
        {
            DisposableList mDisposableList = new DisposableList();

            public void Register<TEvent>(Action<TEvent> onEvent)
            {
                var icanDispose = mConfig.mEventSystem.RegisterEvent<TEvent>(onEvent);
                mDisposableList.Add(icanDispose);
            }

            void UnRegisterAll()
            {
                mDisposableList.Dispose();
            }

            protected override void OnRecycle()
            {
                UnRegisterAll();
            }
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