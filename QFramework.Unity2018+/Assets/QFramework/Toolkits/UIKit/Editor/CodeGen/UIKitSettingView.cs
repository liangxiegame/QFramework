/****************************************************************************
 * Copyright 2019.1 ~ 2020.10 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
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
            
            public static string Apply => IsCN ? "保存" : "Apply";
        }
    }
}