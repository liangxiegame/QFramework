using System.Collections;

namespace QFramework
{
    public interface ICommand : ICanGetModel,ICanGetSystem,ICanGetUtility,ICanSendEvent,ICanSendCommand
    {
        void Execute();
    }

    public class Command
    {
        
    }

    public interface IAsyncCommand
    {
        IEnumerable Execute();
    }
}