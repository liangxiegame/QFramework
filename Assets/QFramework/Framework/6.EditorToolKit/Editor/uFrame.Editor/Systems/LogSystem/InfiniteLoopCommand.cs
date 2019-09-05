using System.ComponentModel;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.LogSystem
{
    public class InfiniteLoopCommand : ICommand, IBackgroundCommand
    {
        public string Title { get;  set; }

        public BackgroundWorker Worker { get; set; }
    }
}