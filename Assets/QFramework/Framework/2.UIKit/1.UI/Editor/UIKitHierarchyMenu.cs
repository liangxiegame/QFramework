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
            var gameObj = Selection.objects.First() as GameObject;

            if (gameObj)
            {
                gameObj.AddComponent<UIMark>();
            }
        }

    }
}