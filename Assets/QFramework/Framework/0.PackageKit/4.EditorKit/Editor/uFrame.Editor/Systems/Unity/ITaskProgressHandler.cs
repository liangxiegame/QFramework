namespace QFramework.GraphDesigner.Unity
{
    public interface ITaskProgressHandler
    {
        void Progress(float progress, string message);
    }
}