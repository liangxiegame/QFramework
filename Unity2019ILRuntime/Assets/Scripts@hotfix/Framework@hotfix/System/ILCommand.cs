namespace QFramework
{
    public interface ILCommand : ILCanGetSystem,ILCanSendCommand,ILCanGetModel,ILCanGetPanel
    {
        void Execute();
    }

    public abstract class ILCommand<TArchitecture> : ILCommand where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        public T GetSystem<T>() where T : class, ILSystem
        {
            return ILSingleton<TArchitecture>.Instance.GetSystem<T>();
        }

        public void SendCommand<T>() where T : ILCommand, new()
        {
            ILSingleton<TArchitecture>.Instance.SendCommand<T>();
        }

        public void SendCommand<T>(T t) where T : ILCommand
        {
            ILSingleton<TArchitecture>.Instance.SendCommand(t);

        }

        public T GetModel<T>() where T : class, ILModel
        {
            return ILSingleton<TArchitecture>.Instance.GetModel<T>();
        }

        public abstract void Execute();
        public T GetPanel<T>() where T : ILUIPanel, new()
        {
            return ILUIKit.GetPanel<T>();
        }
    }
}