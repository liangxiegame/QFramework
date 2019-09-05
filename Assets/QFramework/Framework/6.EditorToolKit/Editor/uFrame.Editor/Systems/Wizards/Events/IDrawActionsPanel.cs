using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI.api
{
    public interface IDrawActionsPanel
    {
        void DrawActionsPanel(IPlatformDrawer platform, Rect bounds, List<ActionItem> actions, Action<ActionItem, Vector2> primaryAction,
            Action<ActionItem, Vector2> secondaryAction = null);
    }
}
