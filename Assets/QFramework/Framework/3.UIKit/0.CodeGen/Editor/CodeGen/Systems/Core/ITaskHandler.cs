using System.Collections;

namespace QFramework.CodeGen
{
    public interface ITaskHandler
    {
        void BeginBackgroundTask(IEnumerator task);
    }
}