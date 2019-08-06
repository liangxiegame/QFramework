using System;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class QuickAddItem :IItem
    {
        private string _searchTag;

        public QuickAddItem(string @group, string title, Action<QuickAddItem> action)
        {
            Group = @group;
            Title = title;
            Action = action;
        }
        public DiagramViewModel Diagram { get; set; }
        public Vector2 MousePosition { get; set; }
        public IDiagramNodeItem Item { get; set; }
        public Action<QuickAddItem> Action { get; set; }

        public virtual string Title { get; set; }

        public string Group { get; set; }

        public string SearchTag
        {
            get { return _searchTag ?? Group + Title; }
            set { _searchTag = value; }
        }

        public string Description { get; set; }
    }
}