using System.Collections.Generic;

namespace Invert.Windows
{
    public class LauncherWindowViewModel
    {

        public List<IWindowFactory> AvailableWindows
        {
            get { return WindowsPlugin.LaucherWindows; }
            set { }
        }

    }
}