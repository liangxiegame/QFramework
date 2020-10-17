/****************************************************************************
 * Copyright (c) 2017 ~ 2019.8 liangxie
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

using System.ComponentModel;
using System.Linq;
using QFramework.PackageKit;

namespace QFramework
{
    using UnityEditor;
    using UnityEngine;

    public class ResKitEditorWindow : EditorWindow
    {
        public static void ForceClear()
        {
            ResKitAssetsMenu.AssetBundlesOutputPath.DeleteDirIfExists();
            (Application.streamingAssetsPath + "/AssetBundles").DeleteDirIfExists();

            AssetDatabase.Refresh();
        }

        [MenuItem("QFramework/Res Kit %#r")]
        public static void ExecuteAssetBundle()
        {
            var window = (ResKitEditorWindow) GetWindow(typeof(ResKitEditorWindow), true);
            Debug.Log(Screen.width + " screen width*****");
            window.position = new Rect(100, 100, 600, 400);
            window.Show();
        }

        private void OnEnable()
        {
            mResKitView = new ResKitView();
            var container = new QFrameworkContainer();
            container.RegisterInstance<EditorWindow>(this);
            mResKitView.Init(container);
        }

        ResKitView mResKitView = null;

        public static bool EnableGenerateClass
        {
            get { return EditorPrefs.GetBool(ResKitView.KEY_AUTOGENERATE_CLASS, false); }
        }

        public void OnDisable()
        {
            mResKitView.Clear();
            mResKitView.OnDispose();
            mResKitView = null;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            mResKitView.DrawGUI();

            GUILayout.EndVertical();
            GUILayout.Space(50);

            // RenderEndCommandExecuter.ExecuteCommand();
        }

        private static void BuildWithTarget(BuildTarget buildTarget)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            BuildScript.BuildAssetBundles(buildTarget);
        }

        [DisplayName("ResKit 设置/编辑器")]
        [PackageKitRenderOrder(2)]
        public class ResKitView : VerticalLayout, IPackageKitView
        {
            public IQFrameworkContainer Container { get; set; }

            public bool Ignore
            {
                get { return false; }
            }

            public bool Enabled
            {
                get { return true; }
            }

            private string mResVersion = "100";
            private bool mEnableGenerateClass = false;

            private int mBuildTargetIndex = 0;

            private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
            public const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";

            public void Init(IQFrameworkContainer container)
            {
                EasyIMGUI.Label().Text(LocaleText.ResKit).FontSize(12).AddTo(this);

                var verticalLayout = new VerticalLayout("box").AddTo(this);

                var persistLine = new HorizontalLayout();
                EasyIMGUI.Label().Text("PersistantPath:").AddTo(persistLine).Width(100);
                EasyIMGUI.TextField().Text(Application.persistentDataPath).AddTo(persistLine);
                persistLine.AddTo(verticalLayout);

                EasyIMGUI.Button()
                    .Text(LocaleText.GoToPersistent)
                    .OnClick(() => { EditorUtility.RevealInFinder(Application.persistentDataPath); })
                    .AddTo(verticalLayout);

                mResVersion = EditorPrefs.GetString(KEY_QAssetBundleBuilder_RESVERSION, "100");
                mEnableGenerateClass = EditorPrefs.GetBool(KEY_AUTOGENERATE_CLASS, true);

                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.WebGL:
                        mBuildTargetIndex = 3;
                        break;
                    case BuildTarget.Android:
                        mBuildTargetIndex = 2;
                        break;
                    case BuildTarget.iOS:
                        mBuildTargetIndex = 1;
                        break;
                    default:
                        mBuildTargetIndex = 0;
                        break;
                }

                EasyIMGUI.Toolbar()
                    .AddMenu("win/osx")
                    .AddMenu("iOS")
                    .AddMenu("Android")
                    .AddMenu("WebGL")
                    .Index(mBuildTargetIndex)
                    .AddTo(verticalLayout);

                EasyIMGUI.Toggle()
                    .Text(LocaleText.AutoGenerateClass)
                    .IsOn(mEnableGenerateClass)
                    .AddTo(verticalLayout)
                    .ValueProperty.Bind(v => mEnableGenerateClass = v);

                EasyIMGUI.Toggle()
                    .Text(LocaleText.SimulationMode)
                    .IsOn(AssetBundleSettings.SimulateAssetBundleInEditor)
                    .AddTo(verticalLayout)
                    .ValueProperty.Bind(v => AssetBundleSettings.SimulateAssetBundleInEditor = v);

                var resVersionLine = new HorizontalLayout()
                    .AddTo(verticalLayout);

                EasyIMGUI.Label().Text("ResVesion:").AddTo(resVersionLine).Width(100);

                EasyIMGUI.TextField().Text(mResVersion).AddTo(resVersionLine)
                    .Content.Bind(v => mResVersion = v);

                EasyIMGUI.Button()
                    .Text(LocaleText.GenerateClass)
                    .OnClick(() =>
                    {
                        BuildScript.WriteClass();
                        AssetDatabase.Refresh();
                    }).AddTo(verticalLayout);

                EasyIMGUI.Button()
                    .Text(LocaleText.Build)
                    .OnClick(() =>
                    {
                        EditorLifecycle.PushCommand(() =>
                        {
                            var window = container.Resolve<EditorWindow>();

                            if (window)
                            {
                                window.Close();
                            }

                            BuildWithTarget(EditorUserBuildSettings.activeBuildTarget);
                        });
                    }).AddTo(verticalLayout);

                EasyIMGUI.Button()
                    .Text(LocaleText.ForceClear)
                    .OnClick(() => { ForceClear(); })
                    .AddTo(verticalLayout);

                verticalLayout.AddChild(EasyIMGUI.Space().Pixel(10));
                verticalLayout.AddChild(EasyIMGUI.Label().Text("已标记 AB 列表:").FontBold().FontSize(15));


                var scrollView = EasyIMGUI.Scroll().AddTo(verticalLayout);
                mMarkedPathList = new VerticalLayout("box")
                    .AddTo(scrollView);

                ReloadMarkedList();
            }

            void ReloadMarkedList()
            {
                mMarkedPathList.Clear();

                AssetDatabase.GetAllAssetBundleNames()
                    .SelectMany(n =>
                    {
                        var result = AssetDatabase.GetAssetPathsFromAssetBundle(n);

                        return result.Select(r =>
                            {
                                if (ResKitAssetsMenu.Marked(r))
                                {
                                    return r;
                                }

                                if (ResKitAssetsMenu.Marked(r.GetPathParentFolder()))
                                {
                                    return r.GetPathParentFolder();
                                }

                                return null;
                            }).Where(r => r != null)
                            .Distinct();
                    })
                    .ForEach(n => new HorizontalLayout()
                        .AddChild(EasyIMGUI.Label().Text(n))
                        .AddChild(EasyIMGUI.Button()
                            .Text("选择")
                            .OnClick(() =>
                            {
                                Selection.objects = new[]
                                {
                                    AssetDatabase.LoadAssetAtPath<Object>(n)
                                };
                            }).Width(50))
                        .AddChild(EasyIMGUI.Button()
                            .Text("取消标记")
                            .OnClick(() =>
                            {
                                ResKitAssetsMenu.MarkAB(n);

                                EditorLifecycle.PushCommand(() => { ReloadMarkedList(); });
                            }).Width(75))
                        .AddTo(mMarkedPathList)
                    );
            }


            private VerticalLayout mMarkedPathList = null;


            void IPackageKitView.OnGUI()
            {
                this.DrawGUI();
            }

            public void OnDispose()
            {
                EditorPrefs.SetBool(KEY_AUTOGENERATE_CLASS, mEnableGenerateClass);
                EditorPrefs.SetString(KEY_QAssetBundleBuilder_RESVERSION, mResVersion);
            }

            public void OnUpdate()
            {
            }
        }

        public class LocaleText
        {
            public static string ResKit
            {
                get { return Language.IsChinese ? "Res Kit 设置" : "Res Kit Setting"; }
            }

            public static string GoToPersistent
            {
                get { return Language.IsChinese ? "打开 Persistent 目录" : "Go To Persistance"; }
            }

            public static string GenerateClass
            {
                get { return Language.IsChinese ? "生成代码（资源名常量）" : "Generate Class"; }
            }

            public static string Build
            {
                get { return Language.IsChinese ? "打 AB 包" : "Build"; }
            }

            public static string ForceClear
            {
                get { return Language.IsChinese ? "清空已生成的 AB" : "ForceClear"; }
            }

            public static string AutoGenerateClass
            {
                get { return Language.IsChinese ? "打 AB 包时，自动生成资源名常量代码" : "auto generate class when build"; }
            }

            public static string SimulationMode
            {
                get
                {
                    return Language.IsChinese
                        ? "模拟模式（勾选后每当资源修改时无需再打 AB 包，开发阶段建议勾选，打真机包时取消勾选并打一次 AB 包）"
                        : "Simulation Mode";
                }
            }
        }
    }
}