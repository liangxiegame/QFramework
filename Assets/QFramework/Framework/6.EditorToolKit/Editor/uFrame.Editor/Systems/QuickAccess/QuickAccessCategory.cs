using System.Collections.Generic;
using QF.GraphDesigner;

public class QuickAccessCategory : ITreeItem
{
    private List<IItem> _childItems;

    public string Title { get; set; }
    public string Group { get; set; }
    public string SearchTag { get; set; }
    public string Description { get; set; }
    public IItem ParentItem { get; private set; }

    public IEnumerable<IItem> Children
    {
        get { return ChildItems; }
    }

    public List<IItem> ChildItems
    {
        get { return _childItems ?? (_childItems = new List<IItem>()); }
        private set { _childItems = value; }
    }

    public bool Expanded { get; set; }

    public void Add(IItem item)
    {
        ChildItems.Add(item);
    }

}