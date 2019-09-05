namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    public interface IMessage
    {
        MessageType MessageType { get; set; }
        string Message { get; set; }
    }
}