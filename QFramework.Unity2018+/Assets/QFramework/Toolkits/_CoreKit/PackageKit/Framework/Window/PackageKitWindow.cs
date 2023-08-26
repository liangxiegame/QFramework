/****************************************************************************
 * Copyright (c) 2016 ~ 2023 liangxie UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QFramework
{
    [InitializeOnLoad]
    public class PackageKitWindow : EasyEditorWindow
    {
        static PackageKitWindow()
        {
            mViews = null;
        }

        [DidReloadScripts]
        public static void Reload()
        {
            var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            if (editorWindows.Any(w => w.GetType() == typeof(PackageKitWindow) && w.As<PackageKitWindow>().Openning))
            {
                GetWindow<PackageKitWindow>().ReInitNextFrame();
            }
        }

        private void OnEnable()
        {
            var _ = LocaleKitEditor.IsCN;
        }

        private static List<IPackageKitView> Views
        {
            get
            {
                if (mViews != null) return mViews;
                mViews = new List<IPackageKitView>();
                InitializeViews(mViews);
                return mViews;
            }
            set => mViews = value;
        }

        private static void InitializeViews(List<IPackageKitView> views)
        {
            views.Clear();

            var viewTypes = PackageKitAssemblyCache.GetDerivedTypes<IPackageKitView>(false, false).ToArray();

            foreach (var view in viewTypes)
            {
                if (view.GetAttribute<PackageKitIgnoreAttribute>(false) == null)
                {
                    var viewInstance = Activator.CreateInstance(view) as IPackageKitView;
                    if (viewInstance == null) continue;
                    views.Add(viewInstance);
                }
            }

            foreach (var view in views)
            {
                view.EditorWindow = EditorWindow.GetWindow<PackageKitWindow>();
                view.Init();
            }
        }

        private static List<IPackageKitView> mViews;

        class LocaleText
        {
            public static string QFrameworkSettings =>
                LocaleKitEditor.IsCN.Value ? "QFramework 设置" : "QFramework Settings";

            public static string Compiling => LocaleKitEditor.IsCN.Value ? "编译中..." : "Compiling...";
            public static string Reloading => LocaleKitEditor.IsCN.Value ? "重载中..." : "Reloading...";
        }

        private const float toolbarHeight = 20;

        class Styles
        {
            public static GUIStyle box = "box";
            public static GUIStyle toolbarSeachTextFieldPopup = "ToolbarSeachTextFieldPopup";
            public static GUIStyle searchCancelButton = "ToolbarSeachCancelButton";
            public static GUIStyle searchCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
            public static GUIStyle foldout = "Foldout";
            public static GUIStyle toolbarDropDown = "ToolbarDropDown";
            public static GUIStyle selectionRect = "SelectionRect";
        }

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        public static void OpenWindow()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);

            if (packageKitWindow.Openning)
            {
                packageKitWindow.Close();
            }
            else
            {
                packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
                packageKitWindow.position = new Rect(50, 50, 1200, 900);
                packageKitWindow.Open();
            }
        }

        private const string URL_FEEDBACK = "https://qframework.cn/community";

        [MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
        private static void Feedback()
        {
            Application.OpenURL(URL_FEEDBACK);
        }

        private Dictionary<MutableTuple<string, bool>, List<PackageKitViewRenderInfo>>
            mPackageKitViewRenderInfos = null;

        public static string SelectedViewName
        {
            get => EditorPrefs.GetString("QF_SELECTED_DISPLAY_NAME", string.Empty);
            set => EditorPrefs.SetString("QF_SELECTED_DISPLAY_NAME", value);
        }

        protected override void Init()
        {
            Views = null;

            RemoveAllChildren();

            mSplitView = null;

            var reflectRenderInfos = AssetDatabase.FindAssets("packagekitconfig t:TextAsset")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => path.EndsWith(".json"))
                .Select(path =>
                {
                    var jsonText = File.ReadAllText(path);

                    var infoNew = JsonUtility.FromJson<PackageKitScriptViewRenderInfo>(jsonText);

                    if (infoNew.Load())
                    {
                        var renderInfo = new PackageKitViewRenderInfo(infoNew)
                        {
                            DisplayName = infoNew.DisplayName,
                            DisplayNameCN = infoNew.DisplayNameCN,
                            DisplayNameEN = infoNew.DisplayNameEN,
                            GroupName = infoNew.GroupName,
                            RenderOrder = infoNew.RenderOrder,
                            Interface =
                            {
                                EditorWindow = this
                            }
                        };
                        renderInfo.Interface.Init();
                        return renderInfo;
                    }


                    return null;
                }).Where(info => info != null);

            mPackageKitViewRenderInfos = Views
                .Select(view => new PackageKitViewRenderInfo(view))
                .Concat(reflectRenderInfos)
                .GroupBy(renderInfo => renderInfo.GroupName)
                .Select(g => new KeyValuePair<MutableTuple<string, bool>, List<PackageKitViewRenderInfo>>(
                    new MutableTuple<string, bool>(g.Key, true),
                    g.OrderBy(renderInfo => renderInfo.RenderOrder).ToList()))
                .ToDictionary(p => p.Key, p => p.Value);

            if (string.IsNullOrEmpty(SelectedViewName))
            {
                mSelectedViewRender = mPackageKitViewRenderInfos.FirstOrDefault().Value.FirstOrDefault();
            }
            else
            {
                mSelectedViewRender = mPackageKitViewRenderInfos
                                          .SelectMany(p => p.Value)
                                          .FirstOrDefault(p => p.Interface.ToString() == SelectedViewName) ??
                                      mPackageKitViewRenderInfos.FirstOrDefault().Value.FirstOrDefault();
            }

            mSelectedViewRender.Interface.OnShow();
        }

        [SerializeField] private VerticalSplitView mSplitView;

        public override void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.Label(LocaleText.Compiling);
                return;
            }

            if (!mInited)
            {
                GUILayout.Label(LocaleText.Reloading);
                base.OnGUI();
                return;
            }

            if (mSplitView == null)
            {
                mSplitView = new VerticalSplitView
                {
                    FirstPan = rect =>
                    {
                        GUILayout.BeginArea(rect);
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        GUILayout.Space(toolbarHeight);
                        GUILayout.EndVertical();
                        if (mSplitView.Expand.Value)
                        {
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("<"))
                            {
                                mSplitView.Expand.Value = false;
                            }
                        }
                        GUILayout.EndHorizontal();
                        LeftSelectView("");
                        GUILayout.EndArea();
                    },
                    SecondPan = rect =>
                    {
                        GUILayout.BeginArea(rect);
                        GUILayout.BeginHorizontal();
                        if (!mSplitView.Expand.Value)
                        {
                            if (GUILayout.Button(">"))
                            {
                                mSplitView.Expand.Value = true;
                            }

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.BeginVertical();
                        GUILayout.Space(toolbarHeight);
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        if (mSelectedViewRender != null)
                        {
                            mSelectedViewRender.Interface.OnGUI();
                        }

                        GUILayout.EndArea();
                    }
                };
            }

            base.OnGUI();

            GUILayout.BeginHorizontal();
            LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor);
            GUILayout.EndHorizontal();

            var r = GUILayoutUtility.GetLastRect();
            mSplitView?.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(position.width, position.height - r.height)));

            mSelectedViewRender?.Interface?.OnWindowGUIEnd();
            RenderEndCommandExecutor.ExecuteCommand();
        }

        private PackageKitViewRenderInfo mSelectedViewRender = null;

        private void LeftSelectView(string search)
        {
            foreach (var packageKitViewRenderInfo in mPackageKitViewRenderInfos.OrderBy(kv => kv.Key.Item1))
            {
                GUILayout.BeginVertical("box");
                if (EditorGUILayout.Foldout(packageKitViewRenderInfo.Key.Item2, packageKitViewRenderInfo.Key.Item1,
                        true))
                {
                    GUILayout.EndVertical();

                    packageKitViewRenderInfo.Key.Item2 = true;

                    foreach (var drawer in packageKitViewRenderInfo.Value)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Space(20);
                        GUILayout.Label(drawer.GetDisplayName());
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        var rect = GUILayoutUtility.GetLastRect();

                        if (mSelectedViewRender == drawer)
                        {
                            GUI.Box(rect, "", Styles.selectionRect);
                        }

                        if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
                        {
                            mSelectedViewRender.Interface.OnHide();
                            mSelectedViewRender = drawer;
                            mSelectedViewRender.Interface.OnShow();
                            SelectedViewName = mSelectedViewRender.Interface.ToString();
                            Event.current.Use();
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    packageKitViewRenderInfo.Key.Item2 = false;
                    GUILayout.EndVertical();
                }
            }
        }

        public override void OnClose()
        {
            if (mPackageKitViewRenderInfos != null)
            {
                mPackageKitViewRenderInfos.SelectMany(r => r.Value)
                    .Where(view => view != null)
                    .ToList()
                    .ForEach(view => view.Interface.OnDispose());
            }

            RemoveAllChildren();
        }

        public override void OnUpdate()
        {
            if (mSelectedViewRender != null)
            {
                mSelectedViewRender.Interface.OnUpdate();
            }
        }

        public static void OpenWindowWithViewType<T>()
        {
            SelectedViewName = typeof(T).FullName;
            OpenWindow();
        }
    }
}
#endif