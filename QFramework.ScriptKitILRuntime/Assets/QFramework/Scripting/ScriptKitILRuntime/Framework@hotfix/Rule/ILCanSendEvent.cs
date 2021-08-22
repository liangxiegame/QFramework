namespace QFramework
{
    public interface ILCanSendEvent
    {
        void SendEvent<T>() where T : new();

        void SendEvent<T>(T t) where T : class;
    }
}