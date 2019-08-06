namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    public interface ILogEvents
    {
        void Log(string message, MessageType type);
        void Log<T>(T message) where T : LogMessage,new();
    }
}