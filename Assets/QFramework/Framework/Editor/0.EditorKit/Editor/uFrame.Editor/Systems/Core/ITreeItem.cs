using System.Collections.Generic;

namespace QFramework.GraphDesigner
{
    public interface ITreeItem : IItem
    {
        IItem ParentItem { get; }
        IEnumerable<IItem> Children { get; }
        bool Expanded { get; set; }
    }
}