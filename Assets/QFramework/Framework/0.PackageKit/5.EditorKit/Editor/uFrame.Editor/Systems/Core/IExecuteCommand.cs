namespace QF.GraphDesigner
{
    public interface IExecuteCommand<in TCommandType> where TCommandType : ICommand
    {
        void Execute(TCommandType command);
    }
}