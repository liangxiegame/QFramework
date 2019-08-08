using UnityEngine;

namespace QF.GraphDesigner
{
    public class NavigateCommand : Command
    {
        public string FilterId { get; set; }
        public string ItemId { get; set; }
        public string GraphId { get; set; }
        public string Workspaceid { get; set; }
        public Vector2 Scroll { get; set; }
    }
}