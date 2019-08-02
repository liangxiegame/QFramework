using UnityEngine;

namespace QF.GraphDesigner
{
    public class ScreenshotNodeViewModel : DiagramNodeViewModel<ScreenshotNode>
    {
        public ScreenshotNodeViewModel(ScreenshotNode graphItemObject, DiagramViewModel diagramViewModel) : base(graphItemObject, diagramViewModel)
        {
        }

        public float Width
        {
            get { return GraphItem.Width; }
            set { GraphItem.Width = Mathf.RoundToInt(value); }
        }
        public float Height
        {
            get { return GraphItem.Height; }
            set { GraphItem.Height = Mathf.RoundToInt(value); }
        }
    }
}