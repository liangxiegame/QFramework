using System;

namespace Invert.Windows
{
    public interface IWindowFactory
    {
        IWindow GetDefaultViewModelObject(string persistedData);
        Type ViewModelType { get; }
        string Identifier { get; set; }
        bool ShowInLauncher { get; set; }
        bool Multiple { get; set; }
        string LauncherTitle { get; set; }
        void SetAreasFor(IWindowDrawer drawer);
        bool RepaintOnUpdate { get; set; }
    } 
}
