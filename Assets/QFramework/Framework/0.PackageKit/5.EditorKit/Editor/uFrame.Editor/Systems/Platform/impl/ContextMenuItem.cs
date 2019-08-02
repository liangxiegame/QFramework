using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class ContextMenuItem
    {
        private string _title;

        public string Title
        {
            get
            {
            
                return _title ?? Command.GetType().Name;
            }
            set { _title = value; }
        }

        public string Path { get; set; }
        public ICommand Command { get; set; }
        public string Group { get; set; }
        public object Order { get; set; }
        public bool Checked { get; set; }
    }
}