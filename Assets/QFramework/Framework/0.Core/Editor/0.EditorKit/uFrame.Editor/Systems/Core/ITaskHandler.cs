using System.Collections;

namespace QFramework.GraphDesigner
{
    public interface ITaskHandler
    {
        void BeginTask(IEnumerator task);
        void BeginBackgroundTask(IEnumerator task);
    }
}