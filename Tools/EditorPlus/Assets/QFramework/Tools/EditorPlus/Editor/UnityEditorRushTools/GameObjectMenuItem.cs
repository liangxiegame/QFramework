// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 10:27
//  *
//  * 取消了对创建Image和Text的支持，请使用Preset功能
//  * Tips：新版本中使用Preset为一些组件做一个默认值直接创建
//  *
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal static class GameObjectMenuItem
    {
        [MenuItem("GameObject/Duplicate - Top &D")]
        private static void Duplicate()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            if (gameObjects.Length == 0)
            {
                return;
            }

            bool isTop = false;
            int nSiblingIndex = 0;
            if (gameObjects.Length == 1)
            {
                isTop = true;
                nSiblingIndex = Selection.activeTransform.GetSiblingIndex();
            }

            Unsupported.DuplicateGameObjectsUsingPasteboard();
            gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                string strName = gameObject.name;
                if (strName.EndsWith(")"))
                {
                    gameObject.name = strName.Remove(strName.Length - 4);
                }

                Undo.RegisterCreatedObjectUndo(gameObject, "Duplicate");
            }

            if (isTop)
            {
                Selection.activeTransform.SetSiblingIndex(nSiblingIndex+1);
            }
        }

        [MenuItem("GameObject/Create Empty - Top &N", false, -1)]
        private static void CreateEmpty()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject go = new GameObject("GameObject");
            Undo.RegisterCreatedObjectUndo(go, "CreateEmpty");
            Selection.activeGameObject = go;
            if (activeTransform) // 移动到选择的物体上
            {
                go.transform.SetParent(activeTransform, false);
                go.transform.SetAsFirstSibling();
                EditorApplication.DirtyHierarchyWindowSorting();
                go.layer = activeTransform.gameObject.layer;

                RectTransform rtTransform = activeTransform.GetComponent<RectTransform>();
                if (rtTransform)
                {
                    go.AddComponent<RectTransform>();
                }
            }
        }

        [MenuItem("GameObject/Transform/Group &G", false)]
        private static void Group()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            if (gameObjects.Length == 0)
            {
                return;
            }

            Transform parent = gameObjects[0].transform.parent;
            int nSiblingIndex = gameObjects[0].transform.GetSiblingIndex();
            GameObject go = new GameObject("Group");
            Undo.RegisterCreatedObjectUndo(go, "CreateEmpty");
            Undo.FlushUndoRecordObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                Undo.SetTransformParent(gameObject.transform, go.transform, "Group");
            }

            go.transform.SetParent(parent);
            go.transform.SetSiblingIndex(nSiblingIndex);
            EditorApplication.DirtyHierarchyWindowSorting();
            EditorGUIUtility.PingObject(gameObjects[0]);
        }

        [MenuItem("GameObject/Transform/Sort &S", false)]
        private static void Sort()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            if (gameObjects.Length == 0)
            {
                return;
            }

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                Transform transform = Selection.transforms[i];
                Sort(transform);
            }

            EditorApplication.DirtyHierarchyWindowSorting();
        }

        private static void Sort(Transform transform)
        {
            Undo.RegisterFullObjectHierarchyUndo(transform, "Sort");
            int count = transform.childCount;
            int nLast = count - 1;
            for (int i = 1; i < count; i++)
            {
                Transform tLast = transform.GetChild(nLast);
                tLast.SetSiblingIndex(i);
                for (int j = 0; j < i; j++)
                {
                    Transform next = transform.GetChild(j);
                    int n = EditorUtility.NaturalCompare(tLast.name, next.name);
                    if (n < 0)
                    {
                        tLast.SetSiblingIndex(j);
                        break;
                    }
                }
            }
        }
    }
}