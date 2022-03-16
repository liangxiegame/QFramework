/****************************************************************************
 * Copyright (c) 2017 ~ 2021.8 liangxiegame MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
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

                    EditorUtility.SetDirty(o);
                    EditorSceneManager.MarkSceneDirty(o.scene);
                }
            }
        }
    }
}