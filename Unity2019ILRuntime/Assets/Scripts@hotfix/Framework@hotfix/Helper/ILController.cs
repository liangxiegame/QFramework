using System;

namespace QFramework
{
    public interface ILController : ILCanSendCommand, ILCanSendEvent, ILCanGetModel, ILCanGetSystem
    {
    }

    public abstract class ILController<TArchitecture> :
        ILController where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public void SendCommand<T>() where T : ILCommand, new()
        {
            ILSingleton<TArchitecture>.Instance.SendCommand<T>();
        }

        public void SendCommand<T>(T t) where T : ILCommand
        {
            ILSingleton<TArchitecture>.Instance.SendCommand(t);
        }

        public void SendEvent<T>() where T : new()
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t) where T : class
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>(t);
        }

        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }

        public T GetSystem<T>() where T : class, ILSystem
        {
            return ILSingleton<TArchitecture>.Instance.GetSystem<T>();
        }
    }

    public class ILControllerNode<TArchitecture> : ILPool<ILControllerNode<TArchitecture>>, ILController
        where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        [Obsolete("请使用 ILControllerNode<TArchitecture>.Allocate()", true)]
        public ILControllerNode()
        {
        }

        protected override void OnRecycle()
        {
        }

        public void SendCommand<T>() where T : ILCommand, new()
        {
            ILSingleton<TArchitecture>.Instance.SendCommand<T>();
        }

        public void SendCommand<T>(T t) where T : ILCommand
        {
            ILSingleton<TArchitecture>.Instance.SendCommand(t);
        }

        public void SendEvent<T>() where T : new()
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t) where T : class
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>(t);
        }

        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }

        public T GetSystem<T>() where T : class, ILSystem
        {
            return ILSingleton<TArchitecture>.Instance.GetSystem<T>();
        }
    }
    
    public abstract class ILViewController<TArchitecture> : ILComponent,
        ILController where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public void SendCommand<T>() where T : ILCommand, new()
        {
            ILSingleton<TArchitecture>.Instance.SendCommand<T>();
        }

        public void SendCommand<T>(T t) where T : ILCommand
        {
            ILSingleton<TArchitecture>.Instance.SendCommand(t);
        }

        public void SendEvent<T>() where T : new()
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t) where T : class
        {
            ILSingleton<TArchitecture>.Instance.SendEvent<T>(t);
        }

        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }

        public T GetSystem<T>() where T : class, ILSystem
        {
            return ILSingleton<TArchitecture>.Instance.GetSystem<T>();
        }
    }
}