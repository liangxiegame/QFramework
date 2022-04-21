using System;

namespace UnityEditor.PackageManager.UI
{
    [Serializable]
    internal enum PackageFilter
    {
        None,
        Unity,
        Local,
        Modules,
        Other
    }
}
