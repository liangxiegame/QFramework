/****************************************************************************
 * Copyright (c) 2018 ~ 2020.12 liangxie
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
        IDisposable RegisterEvent<T>(Action<T> onEvent);

        void RegisterModel<T>(T model) where T : class, IModel;

        void RegisterUtility<T>(T utility) where T : class, IUtility;
        void SendCommand<T>() where T : ICommand, new();

        void SendCommand(ICommand command);

        void SendEvent<T>() where T : new();
        void SendEvent<T>(T t);

        void RegisterSystem<T>(T system) where T : class, ISystem;
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
        }

        void IArchitecture.RegisterModel<T>(T model)
        {
            mContainer.RegisterInstance(model);
        }

        T IArchitecture.GetUtility<T>()
        {
            return mContainer.Resolve<T>();
        }

        void IArchitecture.RegisterUtility<T>(T utility)
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

        void IArchitecture.SendCommand<T>()
        {
            mEventSystem.SendEvent<ICommand>(new T());
        }

        void IArchitecture.SendCommand(ICommand command)
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