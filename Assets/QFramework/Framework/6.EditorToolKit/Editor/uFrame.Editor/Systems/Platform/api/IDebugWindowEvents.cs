using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IDebugWindowEvents
    {
        void RegisterInspectedItem(object item, string name, bool includeReflectiveInspector = false);
        void QuickInspect(object data, string name, Vector2 mousePosition);
        void Watch(object data, string name, Vector2 mousePosition);
    }
}