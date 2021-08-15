namespace QFramework
{
    public interface ILSystem : ILCanGetModel, ILCanGetSystem, ILCanGetUtility, ILCanSendCommand, ILCanSendEvent
    {

    }

    public abstract class ILSystem<TArchitecture> : ILSystem where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }

        public T GetSystem<T>() where T : class, ILSystem
        {
            return ILSingleton<TArchitecture>.Instance.GetSystem<T>();
        }

        public T GetUtility<T>() where T : class, ILUtility
        {
            return ILSingleton<TArchitecture>.Instance.GetUtility<T>();
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
    }
}