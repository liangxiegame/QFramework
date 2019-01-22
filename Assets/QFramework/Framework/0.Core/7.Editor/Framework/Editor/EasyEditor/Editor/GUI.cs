using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditorUI
{
    /// <summary>
    /// Base GUI class. Creates and keeps track of the root of the widget stack, which can then be used to add new widgets.
    /// </summary>
    public class GUI : AbstractLayout
    {
        public GUI() : base(null)
        {
        }
    }
}
