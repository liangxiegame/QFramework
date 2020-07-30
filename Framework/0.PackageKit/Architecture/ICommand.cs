using System.Collections;

namespace QFramework
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IAsyncCommand
    {
        IEnumerable Execute();
    }
}