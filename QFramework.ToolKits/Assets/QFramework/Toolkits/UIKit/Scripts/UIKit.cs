/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections;
using UnityEngine;

namespace QFramework
{
    public class UIKit
    {
        public static UIKitConfig Config = new UIKitConfig();

        public static UIRoot Root => Config.Root;

        /// <summary>
        /// UI 堆栈
        /// </summary>
        public static readonly UIPanelStack Stack = new UIPanelStack();

        /// <summary>
        /// UIPanel  管理（数据结构）
        /// </summary>
        public static readonly UIPanelTable Table = new UIPanelTable();

        public static T OpenPanel<T>(PanelOpenType panelOpenType, UILevel canvasLevel = UILevel.Common,
            IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.OpenType = panelOpenType;
            panelSearchKeys.Level = canvasLevel;
            panelSearchKeys.PanelType = typeof(T);
            panelSearchKeys.AssetBundleName = assetBundleName;
            panelSearchKeys.GameObjName = prefabName;
            panelSearchKeys.UIData = uiData;

            T retPanel = UIManager.Instance.OpenUI(panelSearchKeys) as T;

            panelSearchKeys.Recycle2Cache();

            return retPanel;
        }

        private static WaitForEndOfFrame mWaitForEndOfFrame = new WaitForEndOfFrame();
        
        public static IEnumerator OpenPanelAsync<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.OpenType = PanelOpenType.Single;
            panelSearchKeys.Level = canvasLevel;
            panelSearchKeys.PanelType = typeof(T);
            panelSearchKeys.AssetBundleName = assetBundleName;
            panelSearchKeys.GameObjName = prefabName;
            panelSearchKeys.UIData = uiData;

            bool loaded = false;
            UIManager.Instance.OpenUIAsync(panelSearchKeys, panel =>
            {
                loaded = true;
            });

            while (!loaded)
            {
                yield return mWaitForEndOfFrame;
            }

            panelSearchKeys.Recycle2Cache();
        }

        public static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.OpenType = PanelOpenType.Single;
            panelSearchKeys.Level = canvasLevel;
            panelSearchKeys.PanelType = typeof(T);
            panelSearchKeys.AssetBundleName = assetBundleName;
            panelSearchKeys.GameObjName = prefabName;
            panelSearchKeys.UIData = uiData;

            T retPanel = UIManager.Instance.OpenUI(panelSearchKeys) as T;

            panelSearchKeys.Recycle2Cache();

            return retPanel;
        }

        public static T OpenPanel<T>(IUIData uiData, PanelOpenType panelOpenType = PanelOpenType.Single,
            string assetBundleName = null,
            string prefabName = null) where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.OpenType = panelOpenType;
            panelSearchKeys.Level = UILevel.Common;
            panelSearchKeys.PanelType = typeof(T);
            panelSearchKeys.AssetBundleName = assetBundleName;
            panelSearchKeys.GameObjName = prefabName;
            panelSearchKeys.UIData = uiData;

            T retPanel = UIManager.Instance.OpenUI(panelSearchKeys) as T;

            panelSearchKeys.Recycle2Cache();

            return retPanel;
        }

        public static void ClosePanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.CloseUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void ShowPanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.ShowUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void HidePanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();
            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.HideUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void CloseAllPanel()
        {
            UIManager.Instance.CloseAllUI();
        }

        public static void HideAllPanel()
        {
            UIManager.Instance.HideAllUI();
        }

        public static T GetPanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();
            panelSearchKeys.PanelType = typeof(T);

            var retPanel = UIManager.Instance.GetUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();

            return retPanel as T;
        }

        #region 给脚本层用的 api

        public static UIPanel GetPanel(string panelName)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();
            panelSearchKeys.GameObjName = panelName;

            var retPanel = UIManager.Instance.GetUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();

            return retPanel;
        }

        public static UIPanel OpenPanel(string panelName, UILevel level = UILevel.Common, string assetBundleName = null)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.Level = level;
            panelSearchKeys.AssetBundleName = assetBundleName;
            panelSearchKeys.GameObjName = panelName;

            var retPanel = UIManager.Instance.OpenUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();

            return retPanel as UIPanel;
        }

        public static void ClosePanel(string panelName)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.GameObjName = panelName;

            UIManager.Instance.CloseUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void ClosePanel(UIPanel panel)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.Panel = panel;

            UIManager.Instance.CloseUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void ShowPanel(string panelName)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.GameObjName = panelName;

            UIManager.Instance.ShowUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        public static void HidePanel(string panelName)
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.GameObjName = panelName;

            UIManager.Instance.HideUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

        #endregion

        public static void Back(string currentPanelName)
        {
            if (!string.IsNullOrEmpty(currentPanelName))
            {
                var panelSearchKeys = PanelSearchKeys.Allocate();

                panelSearchKeys.GameObjName = currentPanelName;

                UIManager.Instance.CloseUI(panelSearchKeys);

                panelSearchKeys.Recycle2Cache();
            }

            Stack.Pop();
        }

        public static void Back(UIPanel currentPanel)
        {
            if (currentPanel != null)
            {
                var panelSearchKeys = PanelSearchKeys.Allocate();

                panelSearchKeys.GameObjName = currentPanel.name;

                UIManager.Instance.CloseUI(panelSearchKeys);

                panelSearchKeys.Recycle2Cache();
            }

            Stack.Pop();
        }

        public static void Back<T>()
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.CloseUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();

            Stack.Pop();
        }
    }
}