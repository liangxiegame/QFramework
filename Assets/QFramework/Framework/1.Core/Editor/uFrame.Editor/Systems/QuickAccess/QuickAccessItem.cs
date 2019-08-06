using System;
using QF.GraphDesigner;

public class QuickAccessItem : IItem
{

    public Action<object> Action;

    public QuickAccessItem()
    {
    }

    public QuickAccessItem(string @group, string title, Action<object> action)
    {
        Group = @group;
        Title = title;
        Action = action;
    }

    public QuickAccessItem(string @group, string title, string searchTag, Action<object> action)
    {
        Group = @group;
        Title = title;
        SearchTag = searchTag;
        Action = action;
    }

    public QuickAccessItem(string title, Action<object> action)
    {
        Action = action;
        Title = title;
    }

    public string Title { get; set; }
    public string Group { get; set; }
    public string SearchTag { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }
    public object Item { get; set; }
}