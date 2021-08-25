/****************************************************************************
 * Copyright (c) 2017 ~ 2018.7 Karsion
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{

    internal static class GameObjectMenuItem
    {
        [MenuItem("GameObject/ReplacePrefab &A")]
        private static void ReplacePrefab()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject go = Selection.gameObjects[i];
                PrefabType type = PrefabUtility.GetPrefabType(go);
                switch (type)
                {
                    case PrefabType.PrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                        GameObject gameObject = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
#if UNITY_2018_2_OR_NEWER
                        Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
#else
                        var prefabParent = PrefabUtility.GetPrefabParent(gameObject);
#endif
                        //Undo.RecordObject(prefabParent, "ReplacePrefab");
                        PrefabUtility.ReplacePrefab(gameObject, prefabParent);
                        Undo.RecordObject(gameObject, "ReplacePrefab");
                        PrefabUtility.RevertPrefabInstance(gameObject);
                        break;
                }
            }
        }

        [MenuItem("GameObject/Duplicate - Top &D")]
        private static void Duplicate()
        {
            //GameObject[] gameObjects = Selection.gameObjects;
            //Object[] newObjects = new Object[gameObjects.Length];
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

        //private static void Duplicate()
        //{
        //    GameObject[] gameObjects = Selection.gameObjects;
        //    Object[] newObjects = new Object[gameObjects.Length];
        //    for (int i = 0; i < gameObjects.Length; i++)
        //    {
        //        GameObject gameObject = gameObjects[i];
        //        int nSiblingIndex = gameObject.transform.GetSiblingIndex() + 1;
        //        Selection.activeGameObject = gameObject;
        //        Unsupported.DuplicateGameObjectsUsingPasteboard();
        //        GameObject gameObjectClone = Selection.activeGameObject;
        //        gameObjectClone.name = gameObject.name;
        //        gameObjectClone.transform.SetSiblingIndex(nSiblingIndex);
        //        EditorApplication.DirtyHierarchyWindowSorting();
        //        newObjects[i] = gameObjectClone;
        //        Undo.RegisterCreatedObjectUndo(gameObjectClone, "Duplicate");
        //    }

        //    Selection.objects = newObjects;
        //}

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
            //Debug.Log("Group");
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

        /// <summary>
        ///     第一次创建UI元素时。没有canvas、EventSystem全部要生成，Canvas作为父节点
        ///     之后再空的位置上建UI元素会自己主动加入到Canvas下
        ///     在非UI树下的GameObject上新建UI元素也会 自己主动加入到Canvas下（默认在UI树下）
        ///     加入到指定的UI元素下
        /// </summary>
        [MenuItem("GameObject/UI/img")]
        private static void CreatImages()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject canvasObj = SecurityCheck();
            GameObject img = Image();

            if (!activeTransform) // 在根文件夹创建的， 自己主动移动到 Canvas下
            {
                img.transform.SetParent(canvasObj.transform, false);
                img.layer = canvasObj.layer;
            }
            else
            {
                if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    img.transform.SetParent(canvasObj.transform, false);
                    img.layer = canvasObj.layer;
                }
                else
                {
                    img.transform.SetParent(activeTransform, false);
                    img.layer = activeTransform.gameObject.layer;
                }
            }
        }

        private static GameObject Image()
        {
            GameObject go = new GameObject("img", typeof(Image));
            Undo.RegisterCreatedObjectUndo(go, "Image");
            go.GetComponent<Image>().raycastTarget = false;
            Selection.activeGameObject = go;
            return go;
        }

        [MenuItem("GameObject/UI/txt")]
        private static void CreatTexts()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject canvasObj = SecurityCheck();
            GameObject txt = Text();
            if (!activeTransform) // 在根文件夹创建的。 自己主动移动到 Canvas下
            {
                txt.transform.SetParent(canvasObj.transform, false);
                txt.gameObject.layer = canvasObj.layer;
            }
            else
            {
                if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    txt.transform.SetParent(canvasObj.transform, false);
                    txt.gameObject.layer = canvasObj.layer;
                }
                else
                {
                    txt.transform.SetParent(activeTransform, false);
                    txt.gameObject.layer = activeTransform.gameObject.layer;
                }
            }
        }

        private static GameObject Text()
        {
            GameObject go = new GameObject("txt", typeof(Text));
            Undo.RegisterCreatedObjectUndo(go, "Text");
            Text text = go.GetComponent<Text>();
            text.raycastTarget = false;
            text.supportRichText = false;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            //text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Arts/Fonts/zh_cn.TTF"); // 默认字体
            Selection.activeGameObject = go;
            return go;
        }

        // 假设第一次创建UI元素 可能没有 Canvas、EventSystem对象！
        private static GameObject SecurityCheck()
        {
            Canvas cv = Object.FindObjectOfType<Canvas>();
            GameObject canvas;
            if (!cv)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
                Undo.RegisterCreatedObjectUndo(canvas, "Canvas");
            }
            else
            {
                canvas = cv.gameObject;
            }

            if (!Object.FindObjectOfType<EventSystem>())
            {
                GameObject go = new GameObject("EventSystem", typeof(EventSystem));
                Undo.RegisterCreatedObjectUndo(go, "EventSystem");
            }

            canvas.layer = LayerMask.NameToLayer("UI");
            return canvas;
        }
    }
}