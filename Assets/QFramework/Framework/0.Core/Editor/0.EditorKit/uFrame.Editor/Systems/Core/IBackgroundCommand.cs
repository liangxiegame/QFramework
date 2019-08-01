using System.ComponentModel;

namespace QFramework.GraphDesigner
{
    public interface IBackgroundCommand : ICommand
    {
        BackgroundWorker Worker { get; set; }
    }
}