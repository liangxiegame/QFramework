namespace QFramework
{
    public interface ICanSendEvent
    {
        void SendEvent<T>() where T : new();
        void SendEvent<T>(T t);
    }
}