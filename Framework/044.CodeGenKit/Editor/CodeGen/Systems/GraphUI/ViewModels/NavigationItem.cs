using System;
using UnityEngine;

namespace QFramework.CodeGen
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string SpecializedIcon { get; set; }
        public NavigationItemState State { get; set; }
        public Action<Vector2> NavigationAction { get; set; }
    }
}