namespace QFramework
{
    public interface ICanSendCommand
    {
        void SendCommand<T>() where T : ICommand,new();
        void SendCommand(ICommand command);
    }
}