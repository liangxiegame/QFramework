using System;
using QFramework.GraphDesigner.Systems.GraphUI.api;
using UnityEngine;

namespace QFramework.GraphDesigner.Systems.GraphUI
{
    public interface IDrawActionDialog
    {
        void DrawActionDialog(IPlatformDrawer platform, Rect bounds, ActionItem item, Action cancel = null);
    }
}