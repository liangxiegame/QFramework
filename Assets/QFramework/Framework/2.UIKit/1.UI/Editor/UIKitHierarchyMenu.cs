using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class UIKitHierarchyMenu
    {
        [MenuItem("GameObject/@UI Kit - Add Mark (alt + m) &m",false,-1)]
        public static void AddMark()
        {
            foreach (var o in Selection.objects.OfType<GameObject>())
            {
                if (o)
                {
                    var uiMark = o.GetComponent<UIMark>();

                    if (!uiMark)
                    {
                        o.AddComponent<UIMark>();
                    }
                }
            }
        }
    }
}