using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace QFramework
{
    public class UICreateService
    {
        public static void CreatUIManager(Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            //UIManager
            GameObject UIManagerGo = new GameObject("UIRoot");
            UIManagerGo.layer = LayerMask.NameToLayer("UI");
            QFramework.UIManager UIManager = UIManagerGo.AddComponent<QFramework.UIManager>();

            CreateUICamera(UIManager, 99, referenceResolution, MatchMode, isOnlyUICamera, isVertical);

            ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);

            //保存UIManager
            ReSaveUIManager(UIManagerGo);
        }

        public static void CreateUICamera(QFramework.UIManager UIManager, float cameraDepth, Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            GameObject UIManagerGo = UIManager.gameObject;
            UIManagerGo.AddComponent<RectTransform>();

            var sObj = new SerializedObject(UIManager);

            //挂载点
            GameObject goTmp = null;
            RectTransform rtTmp = null;

            goTmp = new GameObject("Bg");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mBgTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("AnimationUnder");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mAnimationUnderTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Common");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mCommonTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("AnimationOn");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mAnimationOnTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("PopUI");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mPopUITrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Const");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            sObj.FindProperty("mConstTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Toast");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mToastTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Forward");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            sObj.FindProperty("mForwardTrans").objectReferenceValue = rtTmp.gameObject;


            goTmp = new GameObject("Design");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            goTmp.AddComponent<QFramework.Hide>();

            goTmp = new GameObject("EventSystem");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(UIManagerGo.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            goTmp.AddComponent<UnityEngine.EventSystems.EventSystem>();
            goTmp.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            //UIcamera
            GameObject cameraGo = new GameObject("UICamera");
            cameraGo.transform.SetParent(UIManagerGo.transform);
            cameraGo.transform.localPosition = new Vector3(0, 0, -1000);
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.cullingMask = LayerMask.GetMask("UI");
            camera.orthographic = true;
            camera.depth = cameraDepth;
            sObj.FindProperty("mUICamera").objectReferenceValue = camera.gameObject;

            //Canvas
            Canvas canvasComp = UIManagerGo.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceCamera;
            canvasComp.worldCamera = camera;
            canvasComp.sortingOrder = 100;
            sObj.FindProperty("mCanvas").objectReferenceValue = canvasComp.gameObject;

            //UI Raycaster
            sObj.FindProperty("mGraphicRaycaster").objectReferenceValue = UIManagerGo.AddComponent<GraphicRaycaster>();



            //CanvasScaler
            CanvasScaler scaler = UIManagerGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = MatchMode;
            sObj.FindProperty("mCanvasScaler").objectReferenceValue = scaler;

            if (!isOnlyUICamera)
            {
                camera.clearFlags = CameraClearFlags.Depth;
                camera.depth = cameraDepth;
            }
            else
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
            }
            scaler.matchWidthOrHeight = isVertical ? 1 : 0;
            //重新保存
            ReSaveUIManager(UIManagerGo);

            sObj.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ReSaveUIManager(GameObject UIManagerGo)
        {
            string dirPath = Application.dataPath + "/QFrameworkData/UI/Resources";
            string filePath = "Assets/QFrameworkData/UI/Resources/UIRoot.prefab";
            Debug.Log(dirPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            PrefabUtility.CreatePrefab(filePath, UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);
        }

    }
}