using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class ToolbarItem
    {
        public ICommand Command { get; set; }
        public ToolbarPosition Position { get; set; }
        public int Order { get; set; }
        public bool IsDropdown { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }
    }
}