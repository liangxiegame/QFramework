using System.Collections.Generic;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class SelectionMenuCategory : ITreeItem
    {
        private List<IItem> _childItems;
        public string Title { get; set; }
        public string Group { get; private set; }
        public string SearchTag { get; private set; }
        public string Description { get; set; }
        public IItem ParentItem { get; private set; }

        public IEnumerable<IItem> Children
        {
            get { return ChildItems; }
        }

        public bool Expanded { get; set; }

        public void Add(IItem item)
        {
            ChildItems.Add(item);
        }

        public List<IItem> ChildItems
        {
            get { return _childItems ?? (_childItems = new List<IItem>()); }
            set { _childItems = value; }
        }
    }
}