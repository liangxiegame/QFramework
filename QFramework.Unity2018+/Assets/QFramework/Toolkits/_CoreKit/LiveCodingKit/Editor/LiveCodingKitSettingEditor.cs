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
    [PackageKitRenderOrder(10)]
    [DisplayNameCN("LiveCodingKit 设置")]
    [DisplayNameEN("LiveCodingKit Setting")]
    internal class LiveCodingKitSettingEditor : IPackageKitView
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
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                
                GUILayout.Label(LocaleText.Switch,mLabelBold12.Value,GUILayout.Width(40));
                
                LiveCodingKit.Setting.Open = GUILayout.Toggle(LiveCodingKit.Setting.Open,"");

                if (EditorGUI.EndChangeCheck())
                {
                    LiveCodingKitSetting.Load().Save();
                }
                
                GUILayout.EndHorizontal();
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

            public static string Switch =>
                IsCN ? " 开启：" : "Open:";
            

            public static string Apply => IsCN ? "保存" : "Apply";
        }
    }
}
#endif