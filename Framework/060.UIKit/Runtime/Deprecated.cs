using System;
using UnityEngine;

namespace QFramework
{
    public class UIKitDeprecatedConfig
    {
        public const bool FORCE_DEPRECATED = false;
    }

    [Obsolete]
    public static class UIMgr
    {
        #region 高频率调用的 api 只能在 Mono 层使用

        [Obsolete("UIMgr.OpenPanel is depreacated,Please use UIKit.OpenPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            return UIKit.OpenPanel<T>(canvasLevel, uiData, assetBundleName, prefabName);
        }

        [Obsolete("UIMgr.OpenPanel is depreacated,Please use UIKit.OpenPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static T OpenPanel<T>(IUIData uiData, string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            return UIKit.OpenPanel<T>(UILevel.Common, uiData, assetBundleName, prefabName);
        }

        [Obsolete("UIMgr.ClosePanel is deprecated,Please use UIKit.ClosePanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static void ClosePanel<T>() where T : UIPanel
        {
            UIKit.ClosePanel<T>();
        }

        [Obsolete("UIMgr.ShowPanel is deprecated,Please use UIKit.ShowPanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static void ShowPanel<T>() where T : UIPanel
        {
            UIKit.ShowPanel<T>();
        }

        [Obsolete("UIMgr.HidePanel is deprecated,Please use UIKit.HidePanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static void HidePanel<T>() where T : UIPanel
        {
            UIKit.HidePanel<T>();
        }

        [Obsolete("UIMgr.CloseAllPanel is deprecated,Please use UIKit.CloseAllPanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void CloseAllPanel()
        {
            UIManager.Instance.CloseAllUI();
        }

        [Obsolete("UIMgr.HideAllPanel is deprecated,Please use UIKit.HideAllPanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void HideAllPanel()
        {
            UIManager.Instance.HideAllUI();
        }

        [Obsolete("UIMgr.GetPanel is deprecated,Please use UIKit.GetPanel isteand",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        internal static T GetPanel<T>() where T : UIPanel
        {
            return UIKit.GetPanel<T>();
        }

        #endregion

        #region 给脚本层用的 api

        [Obsolete("UIMgr.GetPanel is depreacated,Please use UIKit.GetPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static UIPanel GetPanel(string panelName)
        {
            return UIKit.GetPanel(panelName);
        }

        [Obsolete("UIMgr.OpenPanel is depreacated,Please use UIKit.OpenPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static UIPanel OpenPanel(string panelName, UILevel level = UILevel.Common, string assetBundleName = null)
        {
            return UIKit.OpenPanel(panelName, level, assetBundleName);
        }

        [Obsolete("UIMgr.OpenPanel is depreacated,Please use UIKit.OpenPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static UIPanel OpenPanel(string panelName)
        {
            return UIKit.OpenPanel(panelName);
        }

        [Obsolete("UIMgr.ClosePanel is depreacated,Please use UIKit.ClosePanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void ClosePanel(string panelName)
        {
            UIKit.ClosePanel(panelName);
        }

        [Obsolete("UIMgr.ShowPanel is depreacated,Please use UIKit.ShowPanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void ShowPanel(string panelName)
        {
            UIKit.ShowPanel(panelName);
        }

        [Obsolete("UIMgr.HidePanel is depreacated,Please use UIKit.HidePanel instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void HidePanel(string panelName)
        {
            UIKit.HidePanel(panelName);
        }

        #endregion

        [Obsolete("UIMgr.Push is depreacated,Please use UIKit.Push instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void Push<T>() where T : UIPanel
        {
            UIManager.Instance.Push<T>();
        }

        [Obsolete("UIMgr.Push is depreacated,Please use UIKit.Push instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void Push(UIPanel view)
        {
            UIManager.Instance.Push(view);
        }

        [Obsolete("UIMgr.Camera is depreacated,Please use UIKit.Root.UICamera instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static Camera Camera
        {
            get { return UIKit.Root.UICamera; }
        }

        [Obsolete("UIMgr.SetResolution is depreacated,Please use UIKit.Root.SetResolution instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static void SetResolution(int width, int height, float matchOnWidthOrHeight)
        {
            UIKit.Root.SetResolution(width, height, matchOnWidthOrHeight);
        }

        [Obsolete("UIMgr.SetResolution is depreacated,Please use UIKit.Root.SetResolution instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static Vector2 GetResolution()
        {
            return UIKit.Root.GetResolution();
        }

        [Obsolete("UIMgr.GetMatchOrWidthOrHeight is depreacated,Please use UIKit.Root.GetMatchOrWidthOrHeight instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static float GetMatchOrWidthOrHeight()
        {
            return UIKit.Root.GetMatchOrWidthOrHeight();
        }
    }

    public partial class UIManager
    {
        [RuntimeInitializeOnLoadMethod]
        static void RegisterMsgCenter()
        {
            QMsgCenter.RegisterManagerFactory(QMgrID.UI, () => Instance);
        }

        [Obsolete("UIManager.Instance.Push is depreacated,Please use UIKit.Stack.Push instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void Push<T>() where T : UIPanel
        {
            UIKit.Stack.Push<T>();
        }

        [Obsolete("UIManager.Instance.Push is depreacated,Please use UIKit.Stack.Push instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void Push(IPanel view)
        {
            UIKit.Stack.Push(view);
        }

        [Obsolete("UIManager.Instance.Back is depreacated,Please use UIKit.Back instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void Back(string currentPanelName)
        {
            UIKit.Back(currentPanelName);
        }

        [Obsolete("UIManager.Instance.Back is depreacated,Please use UIKit.Back instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void Back<T>()
        {
            UIKit.Back<T>();
        }

        [Obsolete("UIManager.Instance.Pop is depreacated,Please use UIKit.Stack.Pop instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void Pop()
        {
            UIKit.Stack.Pop();
        }

        [Obsolete("UIManager.Instance.RootCanvas is depreacated,Please use UIKit.Root.Canvas instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public Canvas RootCanvas
        {
            get { return UIKit.Root.Canvas; }
        }

        [Obsolete("UIManager.Instance.UICamera is depreacated,Please use UIKit.Root.Camera instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public Camera UICamera
        {
            get { return UIKit.Root.UICamera; }
        }


        [Obsolete("UIManager.Intance.SetResolution() is depreacated,Please use UIKit.Root.SetResolution() instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void SetResolution(int width, int height)
        {
            UIKit.Root.CanvasScaler.referenceResolution = new Vector2(width, height);
        }

        [Obsolete("UIManager.Instance.GetResolution is depreacated,Please use UIKit.Root.GetResolution() instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public Vector2 GetResolution()
        {
            return UIKit.Root.CanvasScaler.referenceResolution;
        }

        [Obsolete(
            "UIManager.Instance.SetMatchOnWidthOrHeight() is depreacated,Please use UIKit.Root.SetMatchOnWidthOrHeight() instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public void SetMatchOnWidthOrHeight(float heightPercent)
        {
            UIKit.Root.CanvasScaler.matchWidthOrHeight = heightPercent;
        }

        [Obsolete(
            "UIManager.Instance.GetMatchOnWithOrHeight() is depreacated,Please use UIKit.Root.GetMatchOnWithOrHeight() instead",
            UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        public float GetMatchOnWithOrHeight()
        {
            return UIKit.Root.CanvasScaler.matchWidthOrHeight;
        }
    }

    [Obsolete("UIPanelInfo is depreacated,Please use PanelInfo instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
    public class UIPanelInfo : PanelInfo
    {
    }

    public partial interface IPanel
    {
        [Obsolete("PanelInfo is depreacated,Please use Info instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
        PanelInfo PanelInfo { get; set; }
    }

    [Obsolete("UIMark is depreacated,Please use Bind instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
    /// <inheritdoc />
    /// <summary>
    /// UI的标记
    /// </summary>
    public class UIMark : Bind
    {
    }

    [Obsolete("QUIBehaviour is depreacated,Please use UIPanel instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]
    public abstract class QUIBehaviour : UIPanel
    {
    }


    public partial class UIPanel
    {
        [Obsolete("PanelInfo is deprecated,Please use Info instead", UIKitDeprecatedConfig.FORCE_DEPRECATED)]

        public PanelInfo PanelInfo
        {
            get { return Info; }
            set { Info = value; }
        }

        #region 不建议啊使用

        [Obsolete("deprecated", true)]
        protected virtual void InitUI(IUIData uiData = null)
        {
        }

        [Obsolete("deprecated", true)]
        protected virtual void RegisterUIEvent()
        {
        }

        #endregion
    }
}