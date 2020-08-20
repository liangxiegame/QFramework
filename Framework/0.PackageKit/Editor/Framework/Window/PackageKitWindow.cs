using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace QFramework.PackageKit
{
    public class PackageKitWindow : IMGUIEditorWindow
    {
        class LocaleText
        {
            public static string QFrameworkSettings
            {
                get { return Language.IsChinese ? "QFramework 设置" : "QFramework Settings"; }
            }
        }

        private const float toolbarHeight = 20;

        class Styles
        {
            public static GUIStyle box = "box";
            public static GUIStyle in_title = new GUIStyle("IN Title") {fixedHeight = toolbarHeight + 5};
            public static GUIStyle toolbarSeachTextFieldPopup = "ToolbarSeachTextFieldPopup";
            public static GUIStyle searchCancelButton = "ToolbarSeachCancelButton";
            public static GUIStyle searchCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
            public static GUIStyle foldout = "Foldout";
            public static GUIStyle toolbarDropDown = "ToolbarDropDown";
            public static GUIStyle selectionRect = "SelectionRect";
        }

        [MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
        [MenuItem(FrameworkMenuItems.PackageKit, false, FrameworkMenuItemsPriorities.Preferences)]
        private static void Open()
        {
            var packageKitWindow = Create<PackageKitWindow>(true);
            packageKitWindow.titleContent = new GUIContent(LocaleText.QFrameworkSettings);
            packageKitWindow.position = new Rect(50, 100, 1000, 800);
            packageKitWindow.Show();
        }

        private const string URL_FEEDBACK = "http://feathub.com/liangxiegame/QFramework";

        [MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
        private static void Feedback()
        {
            Application.OpenURL(URL_FEEDBACK);
        }

        public override void OnUpdate()
        {
            mPackageKitViewRenderInfos.ForEach(view => view.Interface.OnUpdate());
        }

        public class PacakgeKitViewRenderInfo
        {
            public QFramework.IPackageKitView Interface { get; private set; }
            public string DisplayName { get; private set; }

            public int RenderOrder { get; private set; }

            public PacakgeKitViewRenderInfo(QFramework.IPackageKitView @interface)
            {
                Interface = @interface;

                var displayName = @interface.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), false)
                    .FirstOrDefault() as DisplayNameAttribute;
                DisplayName = displayName != null ? displayName.DisplayName : @interface.GetType().Name;

                var renderOrder = @interface.GetType()
                    .GetCustomAttributes(typeof(PackageKitRenderOrderAttribute), false)
                    .FirstOrDefault() as PackageKitRenderOrderAttribute;

                RenderOrder = renderOrder != null ? renderOrder.Order : int.MaxValue;
            }
        }

        public List<PacakgeKitViewRenderInfo> mPackageKitViewRenderInfos = null;

        protected override void Init()
        {
            var label = GUI.skin.label;

            PackageApplication.Container = null;
            RemoveAllChidren();

            mPackageKitViewRenderInfos = PackageApplication.Container
                .ResolveAll<IPackageKitView>()
                .Select(view => new PacakgeKitViewRenderInfo(view))
                .OrderBy(renderInfo => renderInfo.RenderOrder)
                .ToList();

            mSelectedViewRender = mPackageKitViewRenderInfos.FirstOrDefault();

            PackageApplication.Container.RegisterInstance(this);
            PackageApplication.Container.RegisterInstance<EditorWindow>(this);

            // 创建双屏
            mSplitView = new VerticalSplitView
            {
                fistPan = rect =>
                {
                    GUILayout.BeginArea(rect);
                    GUILayout.BeginVertical();
                    GUILayout.Space(toolbarHeight);
                    GUILayout.EndVertical();
                    LeftSelectView("");
                    GUILayout.EndArea();
                },
                secondPan = rect =>
                {
                    GUILayout.BeginArea(rect);
                    GUILayout.BeginVertical();
                    GUILayout.Space(toolbarHeight);
                    GUILayout.EndVertical();

                    if (mSelectedViewRender != null)
                    {
                        mSelectedViewRender.Interface.OnGUI();
                    }

                    GUILayout.EndArea();
                }
            };
        }

        private VerticalSplitView mSplitView;


        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DateTime.Now.ToLongTimeString(), Styles.selectionRect);

            GUILayout.EndHorizontal();

            var r = GUILayoutUtility.GetLastRect();
            mSplitView.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(position.width, position.height - r.height)));

            RenderEndCommandExecuter.ExecuteCommand();
        }

        private PacakgeKitViewRenderInfo mSelectedViewRender = null;

        private void LeftSelectView(string search)
        {
            for (int i = 0; i < mPackageKitViewRenderInfos.Count; i++)
            {
                var drawer = mPackageKitViewRenderInfos[i];

                GUILayout.BeginHorizontal(Styles.in_title);
                GUILayout.Label(drawer.DisplayName);
                GUILayout.FlexibleSpace();
                // GUILayout.Label("v0.0.1");
                GUILayout.EndHorizontal();
                Rect rect = GUILayoutUtility.GetLastRect();
                if (mSelectedViewRender == drawer)
                {
                    GUI.Box(rect, "", Styles.selectionRect);
                }

                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
                {
                    mSelectedViewRender = drawer;
                    Event.current.Use();
                }

                GUILayout.Label("", Styles.in_title, GUILayout.Height(0));
            }
        }


        public override void OnClose()
        {
            if (mPackageKitViewRenderInfos != null)
            {
                mPackageKitViewRenderInfos.Where(view => view != null).ToList().ForEach(view => view.Interface.OnDispose());
            }

            RemoveAllChidren();
        }
    }
}