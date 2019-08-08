using QF.GraphDesigner;

public static class ITreeItemExtensions
{
    public static int CountVisibleItems(this ITreeItem item)
    {
        var items = 0;

        if (item.Expanded)
            foreach (var childItems in item.Children)
            {
                items++;
                var childTree = childItems as ITreeItem;
                if (childTree != null) items += childTree.CountVisibleItems();
            }

        return items;
    }
}