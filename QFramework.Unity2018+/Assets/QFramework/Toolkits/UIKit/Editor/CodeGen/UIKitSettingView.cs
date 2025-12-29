/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LINCENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFramework
{
    public class UIKitEditorWindow : EditorWindow
    {
        [MenuItem("QFramework/Toolkits/UI Kit %#u")]
        public static void OpenWindow()
        {
            var window = (UIKitEditorWindow)GetWindow(typeof(UIKitEditorWindow), true);
            Debug.Log(Screen.width + " screen width*****");
            window.position = new Rect(100, 100, 600, 400);
            window.Show();
        }


        private void OnEnable()
        {
            mUIKitSettingView = new UIKitSettingView();
            mUIKitSettingView.Init();
        }

        UIKitSettingView mUIKitSettingView = null;


        public void OnDisable()
        {
            mUIKitSettingView.OnDispose();
            mUIKitSettingView = null;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();


            mUIKitSettingView.OnGUI();


            GUILayout.EndVertical();
            GUILayout.Space(50);
        }
    }

    public class UIKitSettingView
    {
        private UIKitSettingData mUiKitSettingData;

        public void Init()
        {
            mUiKitSettingData = UIKitSettingData.Load();

            mAssemblieNames = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.GetName().Name.StartsWith("UnityEngine") &&
                            !a.GetName().Name.StartsWith("UnityEditor") &&
                            !a.GetName().Name.StartsWith("System.") &&
                            a.GetName().Name != "System")
                .Select(a => a.GetName().Name)
                .OrderBy(n => n)
                .ToArray();
        }

        private string[] mAssemblieNames;

        private readonly FluentGUIStyle mLabelBold12 = FluentGUIStyle.Label()
            .FontSize(12)
            .FontBold();


        private readonly FluentGUIStyle mLabel12 = FluentGUIStyle.Label()
            .FontSize(12);

        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Label(LocaleText.UINamespace, mLabel12.Value, GUILayout.Width(200));

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.UINamespace, mLabelBold12.Value, GUILayout.Width(200));

                    mUiKitSettingData.Namespace = EditorGUILayout.TextField(mUiKitSettingData.Namespace);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.UIScriptGenerateDir, mLabelBold12.Value, GUILayout.Width(200));

                    mUiKitSettingData.UIScriptDir = EditorGUILayout.TextField(mUiKitSettingData.UIScriptDir);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.UIPanelPrefabDir, mLabelBold12.Value, GUILayout.Width(200));

                    mUiKitSettingData.UIPrefabDir = EditorGUILayout.TextField(mUiKitSettingData.UIPrefabDir);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.AssembliesToSearchTypes, mLabelBold12.Value, GUILayout.Width(200));

                    GUILayout.BeginVertical();

                    var toDeleteIndex = -1;
                    for (var i = 0; i < mUiKitSettingData.AssemblyNamesToSearch.Count; i++)
                    {
                        var assemblyName = mUiKitSettingData.AssemblyNamesToSearch[i];
                        GUILayout.BeginHorizontal();

                        var index = Array.FindIndex(mAssemblieNames, a => a == assemblyName);

                        if (index == -1)
                        {
                            index = 0;
                        }

                        index = EditorGUILayout.Popup(index, mAssemblieNames);

                        mUiKitSettingData.AssemblyNamesToSearch[i] = mAssemblieNames[index];

                        if (GUILayout.Button("x"))
                        {
                            toDeleteIndex = i;
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    if (toDeleteIndex != -1)
                    {
                        mUiKitSettingData.AssemblyNamesToSearch.RemoveAt(toDeleteIndex);
                    }

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        mUiKitSettingData.AssemblyNamesToSearch.Add("Assembly-CSharp");
                    }

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(6);


                if (GUILayout.Button(LocaleText.Apply))
                {
                    mUiKitSettingData.Save();
                    CodeGenKit.Setting.Save();
                }
            }
            GUILayout.EndVertical();
        }

        public void OnDispose()
        {
        }


        class LocaleText
        {
            public static bool IsCN => LocaleKitEditor.IsCN.Value;
            public static string UINamespace => IsCN ? " UI 命名空间:" : "UI Namespace:";

            public static string UIScriptGenerateDir => IsCN ? " UI 脚本生成路径:" : " UI Scripts Generate Dir:";

            public static string UIPanelPrefabDir => IsCN ? " UIPanel Prefab 路径:" : " UIPanel Prefab Dir:";

            public static string AssembliesToSearchTypes => IsCN ? " 搜索类型的程序集:" : " Assemblies To Search Types:";

            public static string Apply => IsCN ? "保存" : "Apply";
        }
    }
}