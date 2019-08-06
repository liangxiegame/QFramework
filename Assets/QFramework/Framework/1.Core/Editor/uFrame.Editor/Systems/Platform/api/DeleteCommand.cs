using Invert.Data;

namespace QF.GraphDesigner
{
    public class DeleteCommand : Command, IFileSyncCommand
    {
        public IDataRecord[] Item { get; set; }
    }
}