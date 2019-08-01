using Invert.Data;

namespace QFramework.GraphDesigner
{
    public class DeleteCommand : Command, IFileSyncCommand
    {
        public IDataRecord[] Item { get; set; }
    }
}