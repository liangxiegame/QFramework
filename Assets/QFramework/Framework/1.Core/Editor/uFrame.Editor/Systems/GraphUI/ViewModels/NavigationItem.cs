using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string SpecializedIcon { get; set; }
        public string Tooltip { get; set; }
        public NavigationItemState State { get; set; }
        public Action<Vector2> NavigationAction { get; set; }
        public Action<Vector2> NavigationActionSecondary { get; set; }
        public Action<Vector2> CloseAction { get; set; }
    }
}