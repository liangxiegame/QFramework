// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Date: 2018-05-11 16:45
//  ****************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework
{
    /*
-------------------------------------------------------------------------------------------------------------------------------------------

    To define a hot-key, use the following special characters: 

        % (Ctrl on Windows, Cmd on OS X)
        # (Shift)
        & (Alt)
        _ (no key modifiers). 

    For example, to define the hot-key Shift-Alt-g use "#&g". To define the hot-key g and no key modifiers pressed use "_g".

    Some special keys are supported, for example "#LEFT" would map to Shift-left. The keys supported like this are: 

        LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.
    
    eg. [MenuItem("GameObject/UI/img &g")]

-------------------------------------------------------------------------------------------------------------------------------------------
*/

    internal static class CustomMenuItem
    {
        [MenuItem("GameObject/Duplicate - Top &D")]
        private static void Duplicate()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            Object[] newObjects = new Object[gameObjects.Length];
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                int nSiblingIndex = gameObject.transform.GetSiblingIndex() + 1;
                Selection.activeGameObject = gameObject;
                Unsupported.DuplicateGameObjectsUsingPasteboard();
                GameObject gameObjectClone = Selection.activeGameObject;
                gameObjectClone.name = gameObject.name;
                gameObjectClone.transform.SetSiblingIndex(nSiblingIndex);
                newObjects[i] = gameObjectClone;
                Undo.RegisterCreatedObjectUndo(gameObjectClone, "Duplicate");
            }

            Selection.objects = newObjects;
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
                go.Layer(activeTransform.gameObject.layer);

                //跟随父物体是不是RectTransform?
                RectTransform rtTransform = activeTransform.GetComponent<RectTransform>();
                if (rtTransform)
                {
                    go.AddComponent<RectTransform>();
                }
            }
        }

        //private static int nGroupCount = 0;

        [MenuItem("GameObject/Group &G")]
        private static void Group()
        {
            //没选中物体能干啥
            if (Selection.objects.Length == 0)
            {
                return;
            }

            //nGroupCount++;
            //if (nGroupCount < Selection.objects.Length)
            //{
            //    return;
            //}

            //nGroupCount = 0;
            GameObject[] gameObjects = Selection.gameObjects;
            Transform parent = gameObjects[0].transform.parent;
            int nSiblingIndex = gameObjects[0].transform.GetSiblingIndex();
            //new一个gameObject去装选中的物体
            GameObject go = new GameObject("Group");
            Undo.RegisterCreatedObjectUndo(go, "CreateEmpty");
            Undo.FlushUndoRecordObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                Undo.SetTransformParent(gameObject.transform, go.transform, "Group");
            }

            go.transform.Parent(parent).SiblingIndex(nSiblingIndex);
            EditorGUIUtility.PingObject(gameObjects[0]);
        }

        [MenuItem("GameObject/Transform/Sort &S", false, 0)]
        private static void Sort()
        {
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                Transform transform = Selection.transforms[i];
                Sort(transform);
            }
        }

        //不稳定排序
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
        [MenuItem("GameObject/UI/img (Uncheck Raycast Target)")]
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

        [MenuItem("GameObject/UI/txt (Uncheck Raycast Target)")]
        private static void CreatTexts()
        {
            Transform activeTransform = Selection.activeTransform;
            GameObject canvasObj = SecurityCheck();
            GameObject txt = Text();
            if (!activeTransform) // 在根文件夹创建的。 自己主动移动到 Canvas下
            {
                txt.transform.SetParent(canvasObj.transform, false);
                txt.Layer(canvasObj.layer);
            }
            else
            {
                if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    txt.transform.SetParent(canvasObj.transform, false);
                    txt.Layer(canvasObj.layer);
                }
                else
                {
                    txt.transform.SetParent(activeTransform, false);
                    txt.Layer(activeTransform.gameObject.layer);
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
            GameObject goCanvas;
            if (!cv)
            {
                goCanvas = new GameObject("Canvas", typeof(Canvas));
                Undo.RegisterCreatedObjectUndo(goCanvas, "Canvas");
            }
            else
            {
                goCanvas = cv.gameObject;
            }

            if (!Object.FindObjectOfType<EventSystem>())
            {
                GameObject go = new GameObject("EventSystem", typeof(EventSystem));
                Undo.RegisterCreatedObjectUndo(go, "EventSystem");
            }

            goCanvas.Layer("UI");
            return goCanvas;
        }
    }
}