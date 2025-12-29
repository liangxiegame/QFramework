#if UNITY_EDITOR
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [PackageKitGroup("QFramework")]
    [DisplayNameCN("BuildKit 自定义打包")]
    [DisplayNameEN("BuildKit CustomBuild")]
    [PackageKitRenderOrder("BuildKit")]
    public class BuildKitEditor : IPackageKitView
    {
        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
        }

        public class BuildView
        {
            public string DisplayName;
            public IBuildView View;
        }

        public List<BuildView> mViews = new List<BuildView>();

        public void OnShow()
        {
            var interfaceType = typeof(IBuildView);
            var actionType = typeof(IBuildAction);

            mViews.Clear();
            foreach (var type in ReflectionExtension.GetAssemblyCSharpEditor().GetTypes()
                         .Where(t => !t.IsAbstract && interfaceType.IsAssignableFrom(t))
                         .OrderBy(t => actionType.IsAssignableFrom(t)))
            {
                var displayName = "";
                if (type.GetAttribute<DisplayNameAttribute>() != null)
                {
                    displayName = type.GetAttribute<DisplayNameAttribute>().DisplayName;
                }
                else
                {
                    displayName = type.Name;
                }

                var view = type.CreateInstance<IBuildView>();
                view.Init();
                mViews.Add(new BuildView()
                {
                    View = view,
                    DisplayName = displayName
                });
            }
        }

        public void OnUpdate()
        {
        }

        private FluentGUIStyle mActionNameStyle = FluentGUIStyle.Label()
            .FontBold();

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Version:{PlayerSettings.bundleVersion}");
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"BuildTarget:{EditorUserBuildSettings.activeBuildTarget}");
            GUILayout.Space(5);
            GUILayout.EndHorizontal();


            foreach (var buildActionView in mViews)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label(buildActionView.DisplayName, mActionNameStyle);
                buildActionView.View.OnGUI();

                if (buildActionView.View is IBuildAction action)
                {
                    if (GUILayout.Button("构建"))
                    {
                        action.Build();
                        EditorWindow.Close();
                    }
                }

                GUILayout.EndVertical();
            }
        }

        public void OnHide()
        {
            mViews.Clear();
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
        }
    }
}
#endif