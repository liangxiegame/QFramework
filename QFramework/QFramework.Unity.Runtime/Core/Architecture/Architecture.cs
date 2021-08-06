/****************************************************************************
 * Copyright (c) 2018 ~ 2021.4 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public interface IArchitecture : ISingleton, IDisposable
    {
        T GetModel<T>() where T : class, IModel;
        T GetSystem<T>() where T : class, ISystem;

        T GetUtility<T>() where T : class, IUtility;
        IDisposable RegisterEvent<T>(Action<T> onEvent) where T : struct;

        void RegisterSystem<T>(T system) where T : class, ISystem;
        void RegisterModel<T>(T model) where T : class, IModel;

        void RegisterUtility<T>(T utility) where T : class, IUtility;
        void SendCommand<T>() where T : ICommand, new();

        void SendCommand(ICommand command);

        void SendEvent<T>() where T : struct;
        void SendEvent<T>(T t) where T : struct;
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

        T IArchitecture.GetSystem<T>()
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterSystem<T>(T system) where T : class, ISystem
        {
            mContainer.RegisterInstance(system);
            mSystemsCache.Add(system);
            system.SetArchitecture(this);
        }

        public void RegisterModel<T>(T model) where T : class, IModel
        {
            mContainer.RegisterInstance(model);
            mModelsCache.Add(model);
            model.SetArchitecture(this);
        }

        T IArchitecture.GetUtility<T>()
        {
            return mContainer.Resolve<T>();
        }

        public void RegisterUtility<T>(T utility) where T : class,IUtility
        {
            mContainer.RegisterInstance<T>(utility);
        }

        private readonly IQFrameworkContainer mContainer = new QFrameworkContainer();

        protected readonly ITypeEventSystem mEventSystem = new TypeEventSystem();

        T IArchitecture.GetModel<T>()
        {
            return mContainer.Resolve<T>();
        }

        void IArchitecture.SendEvent<T>()
        {
            mEventSystem.SendEvent<T>();
        }

        void IArchitecture.SendEvent<T>(T @event)
        {
            mEventSystem.SendEvent<T>(@event);
        }


        public void OnSingletonInit()
        {
            // 注册命令模式
            RegisterCommand();
            OnUtilityConfig();
            OnModelConfig();

            foreach (var model in mModelsCache)
            {
                model.Init();
            }
            
            OnSystemConfig();

            foreach (var system in mSystemsCache)
            {
                system.Init();
            }

            OnLaunch();
        }

        protected virtual void RegisterCommand()
        {
            mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
        }

        protected virtual void OnCommandExecute(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        protected virtual void UnRegisterCommand()
        {
            mEventSystem.RegisterEvent<ICommand>(OnCommandExecute);
        }

        void IArchitecture.SendCommand<T>()
        {
            mEventSystem.SendEvent<ICommand>(new T());
        }

        void IArchitecture.SendCommand(ICommand command)
        {
            mEventSystem.SendEvent<ICommand>(command);
        }

        private List<IModel> mModelsCache = new List<IModel>();

        private List<ISystem> mSystemsCache = new List<ISystem>();

        protected abstract void OnSystemConfig();

        protected abstract void OnModelConfig();
        protected abstract void OnUtilityConfig();

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

        IDisposable IArchitecture.RegisterEvent<T>(Action<T> onEvent)
        {
            return mEventSystem.RegisterEvent<T>(onEvent);
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