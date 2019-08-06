using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI.api
{
    public interface IDrawDatabasesWizard
    {
        void DrawDatabasesWizard(IPlatformDrawer Drawer, Rect bounds);
    }
}
