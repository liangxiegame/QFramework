namespace QFramework
{
    public interface ISystem : ICanGetModel, ICanGetSystem, ICanGetUtility, ICanSendEvent, ICanSendCommand
    {

    }

    public class System<TConfig> : ISystem where TConfig : Architecture<TConfig>
    {
        public T GetModel<T>() where T : class, IModel
        {
            return SingletonProperty<TConfig>.Instance.GetModel<T>();
        }

        public T GetUtility<T>() where T : class, IUtility
        {
            return SingletonProperty<TConfig>.Instance.GetUtility<T>();
        }

        public void SendCommand<T>() where T : ICommand, new()
        {
            SingletonProperty<TConfig>.Instance.SendCommand<T>();
        }

        public void SendCommand(ICommand command)
        {
            SingletonProperty<TConfig>.Instance.SendCommand(command);
        }

        public void SendEvent<T>() where T : new()
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>();
        }

        public void SendEvent<T>(T t)
        {
            SingletonProperty<TConfig>.Instance.SendEvent<T>(t);
        }
    }
}