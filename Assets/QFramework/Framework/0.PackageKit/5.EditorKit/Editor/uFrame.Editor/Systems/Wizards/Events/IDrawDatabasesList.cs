using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI.api
{
    public interface IDrawDatabasesList
    {
        void DrawDatabasesList(IPlatformDrawer Drawer, Rect bounds, List<DatabasesListItem> items);
    }
}
