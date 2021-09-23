/****************************************************************************
 * Copyright (c) 2017 ~ 2021.4 liangxie
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

namespace QFramework
{
    using UnityEditor;
    using UnityEngine;

    public class ResKitEditorWindow : EditorWindow
    {
        [MenuItem("QFramework/Res Kit %#r")]
        public static void ExecuteAssetBundle()
        {
            var window = (ResKitEditorWindow) GetWindow(typeof(ResKitEditorWindow), true);
            QFramework.Log.I(Screen.width + " screen width*****");
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


        [DisplayName("ResKit 设置/编辑器")]
        [PackageKitGroup("QFramework")]
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
                EasyIMGUI.Label().Text(LocaleText.ResKit).FontSize(12).Parent(this);

                var verticalLayout = new VerticalLayout("box").Parent(this);

                var persistLine = EasyIMGUI.Horizontal();
                EasyIMGUI.Label().Text("PersistantPath:").Parent(persistLine).Width(100);
                EasyIMGUI.TextField().Text(Application.persistentDataPath).Parent(persistLine);
                persistLine.Parent(verticalLayout);

                EasyIMGUI.Button()
                    .Text(LocaleText.GoToPersistent)
                    .OnClick(() => { EditorUtility.RevealInFinder(Application.persistentDataPath); })
                    .Parent(verticalLayout);

                mResVersion = EditorPrefs.GetString(KEY_QAssetBundleBuilder_RESVERSION, "100");
                mEnableGenerateClass = EditorPrefs.GetBool(KEY_AUTOGENERATE_CLASS, true);

                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.WSAPlayer:
                        mBuildTargetIndex = 4;
                        break;
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
                    .AddMenu("Windows/MacOS")
                    .AddMenu("iOS")
                    .AddMenu("Android")
                    .AddMenu("WebGL")
                    .AddMenu("WSAPlayer")
                    .Index(mBuildTargetIndex)
                    .Parent(verticalLayout);

                EasyIMGUI.Toggle()
                    .Text(LocaleText.AutoGenerateClass)
                    .IsOn(mEnableGenerateClass)
                    .Parent(verticalLayout)
                    .ValueProperty.Bind(v => mEnableGenerateClass = v);

                EasyIMGUI.Toggle()
                    .Text(LocaleText.SimulationMode)
                    .IsOn(ResKitEditorAPI.SimulationMode)
                    .Parent(verticalLayout)
                    .ValueProperty.Bind(v => ResKitEditorAPI.SimulationMode = v);

                var resVersionLine = new HorizontalLayout()
                    .Parent(verticalLayout);

                EasyIMGUI.Label().Text("ResVesion:").Parent(resVersionLine).Width(100);

                EasyIMGUI.TextField().Text(mResVersion).Parent(resVersionLine)
                    .Content.Bind(v => mResVersion = v);

                EasyIMGUI.Button()
                    .Text(LocaleText.GenerateClass)
                    .OnClick(() =>
                    {
                        BuildScript.WriteClass();
                        AssetDatabase.Refresh();
                    }).Parent(verticalLayout);

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

                            ResKitEditorAPI.BuildAssetBundles();
                        });
                    }).Parent(verticalLayout);

                EasyIMGUI.Button()
                    .Text(LocaleText.ForceClear)
                    .OnClick(ResKitEditorAPI.ForceClearAssetBundles)
                    .Parent(verticalLayout);

                verticalLayout.AddChild(EasyIMGUI.Space().Pixel(10));
                verticalLayout.AddChild(EasyIMGUI.Label().Text(LocaleText.MarkedAB).FontBold().FontSize(15));
                
                


                var scrollView = EasyIMGUI.Scroll().Parent(verticalLayout);
                mMarkedPathList = new VerticalLayout("box")
                    .Parent(scrollView);

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
                            .Text(LocaleText.Select)
                            .OnClick(() =>
                            {
                                Selection.objects = new[]
                                {
                                    AssetDatabase.LoadAssetAtPath<Object>(n)
                                };
                            }).Width(50))
                        .AddChild(EasyIMGUI.Button()
                            .Text(LocaleText.CancelMark)
                            .OnClick(() =>
                            {
                                ResKitAssetsMenu.MarkAB(n);

                                EditorLifecycle.PushCommand(() => { ReloadMarkedList(); });
                            }).Width(75))
                        .Parent(mMarkedPathList)
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

            public new void OnShow()
            {
            }

            public new void OnHide()
            {
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

            public static string CancelMark
            {
                get
                {
                    return Language.IsChinese
                        ? "取消标记"
                        : "Cancel Mark";
                }
            }

            public static string Select
            {
                get
                {
                    return Language.IsChinese
                        ? "选择"
                        : "Select";
                }
            }

            public static string MarkedAB
            {
                get
                {
                    return Language.IsChinese
                        ? "已标记的 AB"
                        : "Marked AB";
                }
            }
        }
    }
}