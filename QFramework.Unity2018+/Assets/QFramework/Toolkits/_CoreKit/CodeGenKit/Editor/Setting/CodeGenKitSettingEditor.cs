/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(2)]
    [DisplayNameCN("CodeGenKit 设置")]
    [DisplayNameEN("CodegenKit Setting")]
    internal class CodeGenKitSettingEditor : IPackageKitView
    {
        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
        }

        public void OnUpdate()
        {

        }

        private Lazy<GUIStyle> mLabelBold12 = new Lazy<GUIStyle>(() =>
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
        });

        private Lazy<GUIStyle> mLabel12 = new Lazy<GUIStyle>(() =>
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
            };
        });

        public void OnGUI()
        {

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label(LocaleText.ViewControllerNamespace, mLabel12.Value, GUILayout.Width(200));

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.ViewControllerNamespace, mLabelBold12.Value, GUILayout.Width(200));

                    CodeGenKit.Setting.Namespace = EditorGUILayout.TextField(CodeGenKit.Setting.Namespace);

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.ViewControllerScriptGenerateDir, mLabelBold12.Value,
                        GUILayout.Width(220));

                    CodeGenKit.Setting.ScriptDir =
                        EditorGUILayout.TextField(CodeGenKit.Setting.ScriptDir);

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);


                GUILayout.BeginHorizontal();
                {

                    GUILayout.Label(LocaleText.ViewControllerPrefabGenerateDir, mLabelBold12.Value,
                        GUILayout.Width(220));
                    CodeGenKit.Setting.PrefabDir =
                        EditorGUILayout.TextField(CodeGenKit.Setting.PrefabDir);

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                if (GUILayout.Button(LocaleText.Apply))
                {
                    CodeGenKit.Setting.Save();
                }
            }
            GUILayout.EndVertical();
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
        }

        public void OnShow()
        {
        }

        public void OnHide()
        {
        }


        class LocaleText
        {
            public static bool IsCN => LocaleKitEditor.IsCN.Value;

            public static string ViewControllerNamespace =>
                IsCN ? " ViewController 命名空间:" : "ViewController Namespace:";


            public static string ViewControllerScriptGenerateDir =>
                IsCN ? " ViewController 脚本生成路径:" : " ViewController Code Generate Dir:";

            public static string ViewControllerPrefabGenerateDir =>
                IsCN
                    ? " ViewController Prefab 生成路径:"
                    : " ViewController Prefab Generate Dir:";

            public static string Apply => IsCN ? "保存" : "Apply";
        }
    }
}
#endif