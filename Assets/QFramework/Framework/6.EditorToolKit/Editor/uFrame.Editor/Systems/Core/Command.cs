using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class Command : ICommand
    {
        private string _title;

        public string Title
        {
            get { return _title ?? (_title = this.GetType().Name.Replace("Command", string.Empty)); }
            set { _title = value; }
        }
    }
}