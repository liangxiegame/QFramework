using System;
using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner
{
    public class SelectionMenuItem : IItem
    {
        public SelectionMenuItem(IItem dataSource, Action action = null)
        {
            Action = action;
            Title = dataSource.Title;
            Description = dataSource.Description;
            SearchTag = dataSource.SearchTag;
            Group = dataSource.Group;
        }

        public SelectionMenuItem(string group, string title, Action action)
        {
            Group = group;
            Title = title;
            SearchTag = group + title;
            Action = action;
        }

        public string Title { get; set; }
        public string Group { get; set; }
        public string SearchTag { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
}