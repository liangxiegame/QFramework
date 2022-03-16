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
                GUILayout.Label(LocaleText.Namespace, mLabel12.Value, GUILayout.Width(200));

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.Namespace, mLabelBold12.Value, GUILayout.Width(200));

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
                GUILayout.Space(12);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(LocaleText.ViewControllerScriptGenerateDir, mLabelBold12.Value,
                        GUILayout.Width(220));

                    mUiKitSettingData.DefaultViewControllerScriptDir =
                        EditorGUILayout.TextField(mUiKitSettingData.DefaultViewControllerScriptDir);

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);


                GUILayout.BeginHorizontal();
                {

                    GUILayout.Label(LocaleText.ViewControllerPrefabGenerateDir, mLabelBold12.Value,
                        GUILayout.Width(220));
                    mUiKitSettingData.DefaultViewControllerPrefabDir =
                        EditorGUILayout.TextField(mUiKitSettingData.DefaultViewControllerPrefabDir);

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                if (GUILayout.Button(LocaleText.Apply))
                {
                    mUiKitSettingData.Save();
                }
            }
            GUILayout.EndVertical();
        }

        public void OnDispose()
        {
        }
        

        class LocaleText
        {
            public static string Namespace
            {
                get { return Language.IsChinese ? " 默认命名空间:" : " Namespace:"; }
            }

            public static string UIScriptGenerateDir
            {
                get { return Language.IsChinese ? " UI 脚本生成路径:" : " UI Scripts Generate Dir:"; }
            }

            public static string UIPanelPrefabDir
            {
                get { return Language.IsChinese ? " UIPanel Prefab 路径:" : " UIPanel Prefab Dir:"; }
            }

            public static string ViewControllerScriptGenerateDir
            {
                get { return Language.IsChinese ? " ViewController 脚本生成路径:" : " Default ViewController Generate Dir:"; }
            }

            public static string ViewControllerPrefabGenerateDir
            {
                get
                {
                    return Language.IsChinese
                        ? " ViewController Prefab 生成路径:"
                        : " Default ViewController Prefab Dir:";
                }
            }

            public static string Apply
            {
                get { return Language.IsChinese ? "保存" : "Apply"; }
            }
            
        }
    }
    internal class Language
    {
        public static bool IsChinese
        {
            get
            {
                return Application.systemLanguage == SystemLanguage.Chinese ||
                       Application.systemLanguage == SystemLanguage.ChineseSimplified;
            }
        }
    }
}