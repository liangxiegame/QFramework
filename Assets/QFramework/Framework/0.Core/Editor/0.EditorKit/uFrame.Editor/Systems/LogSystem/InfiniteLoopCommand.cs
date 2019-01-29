using System.ComponentModel;
using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner.Unity.LogSystem
{
    public class InfiniteLoopCommand : ICommand, IBackgroundCommand
    {
        public string Title { get;  set; }

        public BackgroundWorker Worker { get; set; }
    }
}