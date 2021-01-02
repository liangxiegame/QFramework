using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Tables
{
    public static class TableLayoutUtilities
    {
        public static GameObject InstantiatePrefab(string name, bool playMode = false, bool generateUndo = true)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/" + name);

            if (prefab == null)
            {
                throw new UnityException(String.Format("Could not find prefab '{0}'!", name));
            }

            Transform parent = null;

#if UNITY_EDITOR
            if (!playMode) parent = UnityEditor.Selection.activeTransform;
#endif
            var gameObject = GameObject.Instantiate(prefab) as GameObject;
            gameObject.name = name;

            if (parent == null || !(parent is RectTransform))
            {
                parent = GetCanvasTransform();
            }

            gameObject.transform.SetParent(parent);

            var transform = (RectTransform)gameObject.transform;
            var prefabTransform = (RectTransform)prefab.transform;

            FixInstanceTransform(prefabTransform, transform);

#if UNITY_EDITOR
            if (generateUndo)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Created " + name);
            }
#endif

            return gameObject;
        }

        public static Transform GetCanvasTransform()
        {
            Canvas canvas = null;
#if UNITY_EDITOR
            // Attempt to locate a canvas object parented to the currently selected object
            if (!Application.isPlaying && UnityEditor.Selection.activeGameObject != null)
            {
                canvas = FindParentOfType<Canvas>(UnityEditor.Selection.activeGameObject);
                //canvas = UnityEditor.Selection.activeTransform.GetComponentInParent<Canvas>();                
            }
#endif

            if (canvas == null)
            {
                // Attempt to find a canvas anywhere
                canvas = UnityEngine.Object.FindObjectOfType<Canvas>();

                if (canvas != null) return canvas.transform;
            }

            // if we reach this point, we haven't been able to locate a canvas
            // ...So I guess we'd better create one


            GameObject canvasGameObject = new GameObject("Canvas");
            canvasGameObject.layer = LayerMask.NameToLayer("UI");
            canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(canvasGameObject, "Create Canvas");
#endif

            var eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemGameObject = new GameObject("EventSystem");
                eventSystem = eventSystemGameObject.AddComponent<EventSystem>();
                eventSystemGameObject.AddComponent<StandaloneInputModule>();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                eventSystemGameObject.AddComponent<TouchInputModule>();
#endif

#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(eventSystemGameObject, "Create EventSystem");
#endif
            }

            return canvas.transform;
        }

        public static void FixInstanceTransform(RectTransform baseTransform, RectTransform instanceTransform)
        {
            //instanceTransform.localPosition = baseTransform.localPosition;
            instanceTransform.localPosition = Vector3.zero;
            //instanceTransform.position = baseTransform.position;
            instanceTransform.position = Vector3.zero;
            instanceTransform.rotation = baseTransform.rotation;
            instanceTransform.localScale = baseTransform.localScale;
            instanceTransform.anchoredPosition3D = new Vector3(baseTransform.anchoredPosition3D.x, baseTransform.anchoredPosition3D.y, 0);
            instanceTransform.sizeDelta = baseTransform.sizeDelta;
        }

        public static T FindParentOfType<T>(GameObject childObject)
            where T : UnityEngine.Object
        {
            Transform t = childObject.transform;
            while (t.parent != null)
            {
                var component = t.parent.GetComponent<T>();

                if (component != null) return component;

                t = t.parent.transform;
            }

            // We didn't find anything
            return null;
        }
    }    
}
