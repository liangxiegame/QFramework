namespace QFramework
{
    public interface ILCanSendCommand
    {
        void SendCommand<T>() where T : ILCommand,new();
        void SendCommand<T>(T t) where T : ILCommand;
    }
    
}