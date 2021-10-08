using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using BDFramework;
using ILRuntime.Runtime.CLRBinding;
using Tool;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [DisplayName("ILRuntime 设置")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(4)]
    public class ScriptKitILRuntimeEditorView : IPackageKitView
    {
        public IQFrameworkContainer Container   { get; set; }
        private VerticalLayout mRootLayout = null;
        private bool showGenAdapter = true;
        private string assemblyName = "Assembly-CSharp";
        private string adapterClassName = "";

        private bool showGenDll = true;
        private bool showGenDllBind = true;

        /// <summary>
        /// 生成类适配器
        /// </summary>
        void GenCrossBindAdapter()
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Type type = null;
            bool isFindAsm = false;
            foreach (var assembly in assemblys)
            {
                var name = assembly.GetName().Name;
                if (name == assemblyName)
                {
                    type = assembly.GetType(adapterClassName);
                    isFindAsm = true;
                    break;
                }
            }

            if (!isFindAsm)
            {
                Debug.Log("程序集名找不到");
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("程序集名找不到"));
                return;
            }

            if (isFindAsm && type == null)
            {
                Debug.Log("类名找不到，检查一下命名空间和名字");
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("类名找不到，检查一下命名空间和名字"));
                return;
            }
            
            GenAdapter.CreateAdapter(type, "Assets/" +  ILRuntimeScriptSetting.Default.GenAdaptorPath);
        }

        //生成clr绑定
        private static void GenClrBindingByAnalysis(RuntimePlatform platform = RuntimePlatform.Lumin)
        {
            if (platform == RuntimePlatform.Lumin)
            {
                platform = Application.platform;
            }

            //用新的分析热更dll调用引用来生成绑定代码
            var dllpath = Application.streamingAssetsPath + "/AssetBundles/" + AssetBundleSettings.GetPlatformForAssetBundles(platform) +
                          "/hotfix.dll";
            ILRuntimeHelper.LoadHotfix(dllpath, false);
            BindingCodeGenerator.GenerateBindingCode(ILRuntimeHelper.AppDomain,
                "Assets/" + ILRuntimeScriptSetting.Default.GenClrBindPath.CreateDirIfNotExists());
            AssetDatabase.Refresh();
        }


        public void Init(IQFrameworkContainer container)
        {

            SerializeHelper.SerializeContainer.RegisterInstance<IJsonSerializer>(new JsonDotnetSerializer());

            mRootLayout = new VerticalLayout();

            EasyIMGUI.Label().Text("ScriptKitILRuntime 的编辑器").FontSize(12).Parent(mRootLayout);

            //EditorStyles.popup.fixedHeight = 30;
            
            var verticalLayout = new VerticalLayout("box").Parent(mRootLayout);

            var versionText = "0";

            verticalLayout.AddChild(new HorizontalLayout()
                .AddChild(EasyIMGUI.Label().Text("版本号(数字):"))
                .AddChild(EasyIMGUI.TextField()
                    .Text(versionText)
                    .Self(text => text.Content.Bind(t => versionText = t)))
            );

            var versionBtn = EasyIMGUI.Button();
            versionBtn.AddLayoutOption(GUILayout.Height(30));
            verticalLayout.AddChild(versionBtn.Text("生成版本信息").OnClick(() =>
            {
                var generatePath = Application.streamingAssetsPath + "/AssetBundles/" +
                                   AssetBundleSettings.GetPlatformForAssetBundles(Application.platform) + "/";

                var filenames = Directory.GetFiles(generatePath);

                new DLLVersion()
                {
                    Assets = filenames.Select(f => f.GetFileName()).ToList(),
                    Version = versionText.ToInt()
                }.SaveJson(generatePath + "/hotfix.json");

                AssetDatabase.Refresh();
            }));

            EasyIMGUI.Custom().OnGUI(() =>
            {
                GUILayout.BeginVertical();
                {
                    showGenDll = EditorGUILayout.BeginFoldoutHeaderGroup(showGenDll, "编译热更dll");
                    if (showGenDll)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("编译dll(Debug)",GUILayout.Height(30)))
                        {
                            ScriptBuildTools.BuildDll(ScriptBuildTools.BuildMode.Debug);
                        }
                        if (GUILayout.Button("编译dll(Release)",GUILayout.Height(30)))
                        {
                            ScriptBuildTools.BuildDll(ScriptBuildTools.BuildMode.Release);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        ILRuntimeScriptSetting.Default.AutoCompile = GUILayout.Toggle(ILRuntimeScriptSetting.Default.AutoCompile, "自动编译");
                        EditorGUILayout.HelpBox("开启自动编译后，写完代码会自动编译热更dll", MessageType.Info);
                        GUILayout.EndHorizontal();
                        GUI.color = Color.green;
                        EditorGUILayout.HelpBox(
                            @"注意事项:
     1.编译服务使用Roslyn,请放心使用
     2.如编译出现报错，请仔细看报错信息,和报错的代码行列,
       一般均为语法错
     3.语法报错原因可能有:主工程访问hotfix中的类, 使用宏
       编译时代码结构发生变化..等等，需要细心的你去发现"
                        , MessageType.Info);
                        GUI.color = GUI.backgroundColor;
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    showGenAdapter = EditorGUILayout.BeginFoldoutHeaderGroup(showGenAdapter, "生成跨域Adapter");
                    if (showGenAdapter)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("程序集名:");
                        assemblyName = GUILayout.TextField(assemblyName);
                        GUILayout.EndHorizontal();
                        EditorGUILayout.HelpBox("类名如果有命名空间需要带上", MessageType.Info);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("类名:");
                        adapterClassName = GUILayout.TextField(adapterClassName);
                        GUILayout.EndHorizontal();
                        if (GUILayout.Button("生成",GUILayout.Height(30)))
                        {
                            GenCrossBindAdapter();
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    
                    showGenDllBind = EditorGUILayout.BeginFoldoutHeaderGroup(showGenDllBind, "Clr Binding And Link");
                    if (showGenDllBind)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("生成Clr绑定(不知道干嘛别点！)",GUILayout.Height(30)))
                        {
                            GenClrBindingByAnalysis();
                        }
                        if (GUILayout.Button("生成Link.xml",GUILayout.Height(30)))
                        {
                            StripCode.GenLinkXml();
                        }
                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                GUILayout.EndVertical();

            }).Parent(verticalLayout);
            
            var runModelPop = new EnumPopupView(ILRuntimeScriptSetting.Default.HotfixRunMode);
            runModelPop.Style.Value.fixedHeight = 30;
            runModelPop.AddLayoutOption(GUILayout.Height(30));
            runModelPop.ValueProperty.Bind(v => ILRuntimeScriptSetting.Default.HotfixRunMode = (HotfixCodeRunMode)v);
            EasyIMGUI.Horizontal().AddChild(EasyIMGUI.Label().Text("运行模式")).AddChild(runModelPop).Parent(mRootLayout);
            EasyIMGUI.Custom().OnGUI(() =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("热更程序集名字");
                ILRuntimeScriptSetting.Default.HotfixAsmdefName =
                    GUILayout.TextField(ILRuntimeScriptSetting.Default.HotfixAsmdefName);
                GUILayout.EndHorizontal();
                if (string.IsNullOrEmpty(ILRuntimeScriptSetting.Default.HotfixAsmdefName))
                {
                    EditorGUILayout.HelpBox("😄↑↑↑程序集名字为空则默认查找@hotfix后缀的程序集", MessageType.Info);
                }
            }).Parent(mRootLayout);
        }

        public void OnUpdate()
        {

        }

        public void OnDispose()
        {
            ILRuntimeScriptSetting.Save();
        }

        public void OnShow()
        {
            
        }

        public void OnHide()
        {
        }

        public void OnGUI()
        {
            mRootLayout.DrawGUI();
        }
    }
}