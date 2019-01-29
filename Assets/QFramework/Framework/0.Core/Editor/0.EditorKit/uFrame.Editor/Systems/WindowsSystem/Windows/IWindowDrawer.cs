using System.Collections.Generic;

namespace Invert.Windows
{
    public interface IWindowDrawer
    {
        string WindowId { get; set; }
        string WindowFactoryId { get; set; }
        string PersistedData { get; set; }
        bool RepaintOnUpdate { get; set; }
        IWindow ViewModel { get; set; }
        List<Area> Drawers { get; set; }
    }
}