using QFramework;
using QFramework.Json;

namespace QFramework.GraphDesigner
{
    public class RedoItem : TransactionItem
    {
        private string _undoData;
        [JsonProperty]
        public string UndoData
        {
            get { return _undoData; }
            set
            {
                _undoData = value;
                Changed = true;
            }
        }
    }
}