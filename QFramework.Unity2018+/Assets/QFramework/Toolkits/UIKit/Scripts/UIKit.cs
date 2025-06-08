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
#if UNITY_EDITOR
    [ClassAPI("08.UIKit", "UIKit", 0, "UIKit")]
    [APIDescriptionCN("界面管理方案")]
    [APIDescriptionEN("UI Managements Solution")]
#endif
    public class UIKit
    {

        public static UIKitConfig Config = new UIKitConfig();
        
        /// <summary>
        /// UIPanel  管理（数据结构）
        /// </summary>
        public static UIPanelTable Table { get; } = new UIPanelTable();


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("打开界面")]
        [APIDescriptionEN("Open UI Panel")]
        [APIExampleCode(@"

UIKit.OpenPanel<UIHomePanel>();

UIKit.OpenPanel(""UIHomePanel"");
 
UIKit.OpenPanel<UIHomePanel>(prefabName:""UIHomePanelPrefab"");

UIKit.OpenPanel<UIHomePanel>(new UIHomePanelData()
{
    OpenFrom = ""GameOverPanel""
});   


UIKit.OpenPanel<UIHomePanel>(UILevel.Common);  
")]
#endif
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

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("异步打开界面")]
        [APIDescriptionEN("Open UI Panel Async")]
        [APIExampleCode(@"
yield return UIKit.OpenPanelAsync<UIHomePanel>();


// ActionKit Mode
UIKit.OpenPanelAsync<UIHomePanel>().ToAction().Start(this);
")]
#endif
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
            UIManager.Instance.OpenUIAsync(panelSearchKeys, panel => { loaded = true; });

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

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("关闭界面")]
        [APIDescriptionEN("Close Panel")]
        [APIExampleCode(@"
UIKit.ClosePanel<UIHomePanel>();

UIKit.ClosePanel(""UIHomePanel"");
")]
#endif
        public static void ClosePanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.CloseUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("显示界面")]
        [APIDescriptionEN("Show Panel")]
        [APIExampleCode(@"
UIKit.ShowPanel<UIHomePanel>();

UIKit.ShowPanel(""UIHomePanel"");
")]
#endif
        public static void ShowPanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();

            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.ShowUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("隐藏界面")]
        [APIDescriptionEN("Hide Panel")]
        [APIExampleCode(@"
UIKit.HidePanel<UIHomePanel>();

UIKit.HidePanel(""UIHomePanel"");
")]
#endif
        public static void HidePanel<T>() where T : UIPanel
        {
            var panelSearchKeys = PanelSearchKeys.Allocate();
            panelSearchKeys.PanelType = typeof(T);

            UIManager.Instance.HideUI(panelSearchKeys);

            panelSearchKeys.Recycle2Cache();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("关闭全部界面")]
        [APIDescriptionEN("Close All Panel")]
        [APIExampleCode(@"
UIKit.CloseAllPanel();
")]
#endif
        public static void CloseAllPanel()
        {
            UIManager.Instance.CloseAllUI();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("隐藏全部界面")]
        [APIDescriptionEN("Hide All Panel")]
        [APIExampleCode(@"
UIKit.HideAllPanel();
")]
#endif
        public static void HideAllPanel()
        {
            UIManager.Instance.HideAllUI();
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("获取界面")]
        [APIDescriptionEN("Get Panel")]
        [APIExampleCode(@"
var homePanel = UIKit.GetPanel<UIHomePanel>();


UIKit.GetPanel(""UIHomePanel"");
")]
#endif
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

#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("UIKit 界面根节点")]
        [APIDescriptionEN("UIKit Root GameObject")]
        [APIExampleCode(@"
UIKit.Root.SetResolution(1920,1080,0);
")]
#endif
        public static UIRoot Root => Config.Root;

#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("UIKit 界面堆栈")]
        [APIDescriptionEN("UIKit Panel Stack")]
        [APIExampleCode(@"
UIKit.Stack.Push(UIKit.OpenPanel<UIHomePanel>(); // push and close uihomepanel
 
UIKit.Stack.Pop() // pop and open uihomepanel
")]
#endif
        public static UIPanelStack Stack { get; } = new UIPanelStack();



#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("关闭掉当前界面,返回上一个 Push 过的界面")]
        [APIDescriptionEN("Close Current Panel and Back to previous pushed Panel")]
        [APIExampleCode(@"

UIKit.Stack.Push(UIKit.OpenPanel<UIHomePanel>());

var basicPanel = UIKit.OpenPanel<UIBasicPanel>();

UIKit.Back(basicPanel);

// UIHomePanel Opened
")]
#endif
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