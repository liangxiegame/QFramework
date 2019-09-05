using System;
using QF.GraphDesigner.Systems.GraphUI.api;
using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI
{
    public interface IDrawActionDialog
    {
        void DrawActionDialog(IPlatformDrawer platform, Rect bounds, ActionItem item, System.Action cancel = null);
    }
}