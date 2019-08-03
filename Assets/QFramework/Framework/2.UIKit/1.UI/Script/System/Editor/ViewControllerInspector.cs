using System;
using EGO.Framework;
using QF;
using UnityEditor;
using UnityEngine;
using QF.Extensions;

namespace QFramework
{
    [CustomEditor(typeof(ViewController), true)]
    public class ViewControllerInspector : UnityEditor.Editor
    {
        class LocaleText
        {
            public static string ScriptName
            {
                get { return Language.IsChinese ? "生成脚本名:" : "Script name:"; }
            }

            public static string ScriptsFolder
            {
                get { return Language.IsChinese ? "脚本生成目录:" : "Scripts Generate Folder:"; }
            }

            public static string GeneratePrefab
            {
                get { return Language.IsChinese ? "生成 Prefab" : "Genreate Prefab"; }
            }

            public static string PrefabGenreateFolder
            {
                get { return Language.IsChinese ? "Prefab 生成目录:" : "Prefab Generate Folder:"; }
            }
            
            public static string Generate
            {
                get { return Language.IsChinese ? " 生成代码" : " Generate Code"; }
            }
        }


        private VerticalLayout mRootLayout;
        
        private void OnEnable()
        {
            mRootLayout = new VerticalLayout();
            
            new ButtonView(LocaleText.Generate , () =>
                {
                    CreateViewControllerCode.DoCreateCodeFromScene((target as ViewController).gameObject);
                })
                .Height(30)
                .AddTo(mRootLayout);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var codeGenerateInfo = target as ViewController;

            GUILayout.BeginVertical("box");

            GUILayout.Label("代码生成部分", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            });

            if (codeGenerateInfo.ScriptName.IsNullOrEmpty())
            {
                codeGenerateInfo.ScriptName = codeGenerateInfo.name;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptName, GUILayout.Width(150));
            codeGenerateInfo.ScriptName = GUILayout.TextArea(codeGenerateInfo.ScriptName);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptsFolder, GUILayout.Width(150));
            codeGenerateInfo.ScriptsFolder = GUILayout.TextArea(codeGenerateInfo.ScriptsFolder, GUILayout.Height(30));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            codeGenerateInfo.GeneratePrefab =
                GUILayout.Toggle(codeGenerateInfo.GeneratePrefab, LocaleText.GeneratePrefab);
            GUILayout.EndHorizontal();


            if (codeGenerateInfo.GeneratePrefab)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.PrefabGenreateFolder, GUILayout.Width(150));
                codeGenerateInfo.PrefabFolder =
                    GUILayout.TextArea(codeGenerateInfo.PrefabFolder, GUILayout.Height(30));
                GUILayout.EndHorizontal();
            }
            
            mRootLayout.DrawGUI();

            GUILayout.EndVertical();
        }
    }
}