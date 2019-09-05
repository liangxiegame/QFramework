using UnityEngine;

namespace QF.GraphDesigner
{
    public class TabViewModel : ViewModel
    {
        public string Name { get; set; }
        public string[] Path { get; set; }
        public Rect Bounds { get; set; }
        public IGraphData Graph { get; set; }
    }
}