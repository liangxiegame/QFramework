using System.Collections;

namespace QF.GraphDesigner
{
    public interface ITaskHandler
    {
        void BeginTask(IEnumerator task);
        void BeginBackgroundTask(IEnumerator task);
    }
}