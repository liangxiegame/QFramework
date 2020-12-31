/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 ~ 2020.10  liangxie
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

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(ViewController), true)]
    public class ViewControllerInspector : Editor
    {
        class LocaleText
        {
            public static string Namespace
            {
                get { return Language.IsChinese ? "命名空间:" : "Namespace :"; }
            }

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

        private ViewController mCodeGenerateInfo
        {
            get { return target as ViewController; }
        }

        private void OnEnable()
        {
            mRootLayout = new VerticalLayout();

            EasyIMGUI.Button()
                .Text(LocaleText.Generate)
                .OnClick(() =>
                {
                    CreateViewControllerCode.DoCreateCodeFromScene(((ViewController) target).gameObject);
                    GUIUtility.ExitGUI();
                })
                .Height(30)
                .Parent(mRootLayout);

            if (mCodeGenerateInfo.ScriptsFolder.IsNullOrEmpty())
            {
                var setting = UIKitSettingData.Load();
                mCodeGenerateInfo.ScriptsFolder = "Assets" + setting.DefaultViewControllerScriptDir;
            }

            if (mCodeGenerateInfo.PrefabFolder.IsNullOrEmpty())
            {
                var setting = UIKitSettingData.Load();
                mCodeGenerateInfo.PrefabFolder = "Assets" + setting.DefaultViewControllerPrefabDir;
            }

            if (mCodeGenerateInfo.ScriptName.IsNullOrEmpty())
            {
                mCodeGenerateInfo.ScriptName = mCodeGenerateInfo.name;
            }

            if (mCodeGenerateInfo.Namespace.IsNullOrEmpty())
            {
                var setting = UIKitSettingData.Load();
                mCodeGenerateInfo.Namespace = setting.Namespace;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginVertical("box");

            GUILayout.Label("代码生成部分", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            });

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.Namespace, GUILayout.Width(150));
            mCodeGenerateInfo.Namespace = EditorGUILayout.TextArea(mCodeGenerateInfo.Namespace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptName, GUILayout.Width(150));
            mCodeGenerateInfo.ScriptName = EditorGUILayout.TextArea(mCodeGenerateInfo.ScriptName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.ScriptsFolder, GUILayout.Width(150));
            mCodeGenerateInfo.ScriptsFolder =
                EditorGUILayout.TextArea(mCodeGenerateInfo.ScriptsFolder, GUILayout.Height(30));

            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("请将要生成脚本的文件夹拖到下边区域 或 自行填写目录到上一栏中");
            var sfxPathRect = EditorGUILayout.GetControlRect();
            sfxPathRect.height = 200;
            GUI.Box(sfxPathRect, string.Empty);
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(185));
            if (
                Event.current.type == EventType.DragUpdated
                && sfxPathRect.Contains(Event.current.mousePosition)
            )
            {
                //改变鼠标的外表  
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    if (DragAndDrop.paths[0] != "")
                    {
                        var newPath = DragAndDrop.paths[0];
                        mCodeGenerateInfo.ScriptsFolder = newPath;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }


            GUILayout.BeginHorizontal();
            mCodeGenerateInfo.GeneratePrefab =
                GUILayout.Toggle(mCodeGenerateInfo.GeneratePrefab, LocaleText.GeneratePrefab);
            GUILayout.EndHorizontal();

            if (mCodeGenerateInfo.GeneratePrefab)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.PrefabGenreateFolder, GUILayout.Width(150));
                mCodeGenerateInfo.PrefabFolder =
                    GUILayout.TextArea(mCodeGenerateInfo.PrefabFolder, GUILayout.Height(30));
                GUILayout.EndHorizontal();
            }

            var fileFullPath = mCodeGenerateInfo.ScriptsFolder + "/" + mCodeGenerateInfo.ScriptName + ".cs";
            if (File.Exists(mCodeGenerateInfo.ScriptsFolder + "/" + mCodeGenerateInfo.ScriptName + ".cs"))
            {
                var scriptObject = AssetDatabase.LoadAssetAtPath<MonoScript>(fileFullPath);
                if (GUILayout.Button("打开脚本", GUILayout.Height(30)))
                {
                    AssetDatabase.OpenAsset(scriptObject);
                }

                if (GUILayout.Button("选择脚本", GUILayout.Height(30)))
                {
                    Selection.objects = new Object[] {scriptObject};
                }
            }

            mRootLayout.DrawGUI();

            GUILayout.EndVertical();
        }
    }
}