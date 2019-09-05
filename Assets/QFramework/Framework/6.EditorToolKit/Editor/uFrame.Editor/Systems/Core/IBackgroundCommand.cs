using System.ComponentModel;

namespace QF.GraphDesigner
{
    public interface IBackgroundCommand : ICommand
    {
        BackgroundWorker Worker { get; set; }
    }
}