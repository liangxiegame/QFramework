using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public interface IImmediateControlsDrawer<TControl> : IPlatformDrawer
    {
        List<string> ControlsLeftOver { get; set; }
        Dictionary<string, TControl> Controls { get; set; }
        void AddControlToCanvas(TControl control);
        void RemoveControlFromCanvas(TControl control);
        void SetControlPosition(TControl control, float x, float y, float width, float height);
    }
}