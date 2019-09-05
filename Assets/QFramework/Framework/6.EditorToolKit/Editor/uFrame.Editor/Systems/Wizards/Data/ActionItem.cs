using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class ActionItem
    {
        public ICommand Command { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Verb { get; set; }
    }
}