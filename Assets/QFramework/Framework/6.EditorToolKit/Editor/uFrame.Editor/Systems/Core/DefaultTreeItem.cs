using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class DefaultTreeItem : DefaultItem, ITreeItem
    {
        public DefaultTreeItem(string title, string @group) : base(title, @group)
        {
        }

        public IItem ParentItem { get; set; }
        public IEnumerable<IItem> Children { get; set; }
        public bool Expanded { get; set; }
    }
}