using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class UIKitHierarchyMenu
    {
        [MenuItem("GameObject/@UI Kit - Add Bind (alt + b) &b",false,-1)]
        public static void AddBind()
        {
            foreach (var o in Selection.objects.OfType<GameObject>())
            {
                if (o)
                {
                    var uiMark = o.GetComponent<Bind>();

                    if (!uiMark)
                    {
                        o.AddComponent<Bind>();
                    }
                    
                    EditorUtils.MarkCurrentSceneDirty();
                }
            }
        }
    }
}