namespace QF.GraphDesigner.Unity
{
    public interface ITaskProgressEvent
    {
        void Progress(float progress, string message, bool modal);
    }
}