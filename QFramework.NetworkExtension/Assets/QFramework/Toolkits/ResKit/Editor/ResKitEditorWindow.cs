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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QFramework
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;


    public class ResKitEditorWindow : EditorWindow
    {
        private static EncryptConfig mConfigInstance = null;

        static EncryptConfig GetConfig()
        {
            if (mConfigInstance == null)
            {
                TextAsset text = Resources.Load<TextAsset>("EncryptConfig");
                if (text)
                {
                    mConfigInstance = JsonUtility.FromJson<EncryptConfig>(text.text);

                    if (mConfigInstance == null)
                    {
                        mConfigInstance = new EncryptConfig();
                    }
                }
                else
                {
                    mConfigInstance = new EncryptConfig();

                    string savePath = Application.dataPath + "QFrameworkData/Resources/EncryptConfig.Json";
                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(JsonUtility.ToJson(mConfigInstance));
                        }
                    }
                }
            }

            return mConfigInstance;
        }

        [MenuItem("QFramework/Toolkits/Res Kit %#r")]
        public static void OpenWindow()
        {
            var window = (ResKitEditorWindow)GetWindow(typeof(ResKitEditorWindow), true);
            Debug.Log(Screen.width + " screen width*****");
            window.position = new Rect(100, 100, 600, 400);
            window.Show();
        }


        private void OnEnable()
        {
            mResKitView = new ResKitView();
            mResKitView.EditorWindow = this;
            mResKitView.Init();
        }

        ResKitView mResKitView = null;

        public static bool EnableGenerateClass
        {
            get { return EditorPrefs.GetBool(ResKitView.KEY_AUTOGENERATE_CLASS, false); }
        }

        public void OnDisable()
        {
            //string savePath = Application.dataPath + "/QFrameworkData/Resources/EncryptConfig.Json";
            //using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            //    {
            //        sw.WriteLine(JsonUtility.ToJson(GetConfig()));
            //    }
            //}
            //AssetDatabase.Refresh();

            mResKitView.OnDispose();
            mResKitView = null;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();


            mResKitView.OnGUI();


            GUILayout.EndVertical();
            GUILayout.Space(50);

            
            
            // RenderEndCommandExecuter.ExecuteCommand();
        }
    }


    public class ResKitView
    {
        private string mResVersion = "100";
        private bool mEnableGenerateClass = false;

        private int mBuildTargetIndex = 0;

        private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
        public const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";


        public void Init()
        {
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
        }


        private Vector2 mScrollViewPosition;

        private readonly Lazy<GUIStyle> mMarkABStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold
        });

        private string[] mBuildTargets = new string[]
        {
            "Windows/MacOS",
            "iOS",
            "Android",
            "WebGL",
            "WSAPlayer"
        };

        private readonly Lazy<GUIStyle> mResKitNameStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        });
        
        public EditorWindow EditorWindow { get; set; }

        public void OnGUI()
        {
            GUILayout.Label(LocaleText.ResKit, mResKitNameStyle.Value);


            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("PresistentPath:", GUILayout.Width(100));
            GUILayout.TextField(Application.persistentDataPath);
            GUILayout.EndHorizontal();


            if (GUILayout.Button(LocaleText.GoToPersistent))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }

            mBuildTargetIndex = GUILayout.Toolbar(mBuildTargetIndex, mBuildTargets);

            mEnableGenerateClass = GUILayout.Toggle(mEnableGenerateClass, LocaleText.AutoGenerateClass);
            ResKitEditorAPI.SimulationMode =
                GUILayout.Toggle(ResKitEditorAPI.SimulationMode, LocaleText.SimulationMode);

            // EasyIMGUI.Toggle()
            //    .Text(LocaleText.EncryptAB)
            //    .IsOn(GetConfig().EncryptAB)
            //    .Parent(verticalLayout)
            //    .ValueProperty.Bind(v => GetConfig().EncryptAB = v);


            // var aesLine = EasyIMGUI.Horizontal();
            // EasyIMGUI.Label().Text("AES秘钥:").Parent(aesLine).Width(100);
            // EasyIMGUI.TextField().Text(GetConfig().AESKey).Parent(aesLine).Content.OnValueChanged.AddListener(_=>GetConfig().AESKey=_);
            // aesLine.Parent(verticalLayout);

            // EasyIMGUI.Toggle()
            //    .Text(LocaleText.EncryptKey)
            //    .IsOn(GetConfig().EncryptKey)
            //    .Parent(verticalLayout)
            //    .ValueProperty.Bind(v => GetConfig().EncryptKey = v);

            GUILayout.BeginHorizontal();
            mResVersion = EditorGUILayout.TextField(mResVersion);
            GUILayout.EndHorizontal();

            if (GUILayout.Button(LocaleText.GenerateClass))
            {
                BuildScript.WriteClass();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button(LocaleText.Build))
            {
                EditorLifecycle.PushCommand(() =>
                {
                    if (EditorWindow)
                    {
                        EditorWindow.Close();
                    }
                    // var window = container.Resolve<EditorWindow>();
                    //
                    // if (window)
                    // {
                    //     window.Close();
                    // }

                    ResKitEditorAPI.BuildAssetBundles();
                    //if (GetConfig().EncryptAB)
                    //{
                    //    string key = GetConfig().EncryptKey ? RSA.RSAEncrypt("", GetConfig().AESKey): GetConfig().AESKey;
                    //    BundleHotFix.EncryptAB(key);
                    //}
                });
            }

            if (GUILayout.Button(LocaleText.ForceClear))
            {
                ResKitEditorAPI.ForceClearAssetBundles();
            }

            GUILayout.Space(10);

            GUILayout.Label(LocaleText.MarkedAb, mMarkABStyle.Value);

            mScrollViewPosition = GUILayout.BeginScrollView(mScrollViewPosition);
            {
                GUILayout.BeginVertical("box");
                {
                    foreach (var n in AssetDatabase.GetAllAssetBundleNames()
                        .SelectMany(n =>
                        {
                            var result = AssetDatabase.GetAssetPathsFromAssetBundle(n);

                            return result.Select(r =>
                                {
                                    if (ResKitAssetsMenu.Marked(r))
                                    {
                                        return r;
                                    }

                                    if (ResKitAssetsMenu.Marked(r.GetFolderPath()))
                                    {
                                        return r.GetFolderPath();
                                    }

                                    return null;
                                }).Where(r => r != null)
                                .Distinct();
                        }))
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(n);

                            if (GUILayout.Button(LocaleText.Select, GUILayout.Width(50)))
                            {
                                Selection.objects = new[]
                                {
                                    AssetDatabase.LoadAssetAtPath<Object>(n)
                                };
                            }

                            if (GUILayout.Button(LocaleText.CancelMark, GUILayout.Width(75)))
                            {
                                ResKitAssetsMenu.MarkAB(n);

                                // EditorLifecycle.PushCommand(() => { ReloadMarkedList(); });
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void OnDispose()
        {
            EditorPrefs.SetBool(KEY_AUTOGENERATE_CLASS, mEnableGenerateClass);
            EditorPrefs.SetString(KEY_QAssetBundleBuilder_RESVERSION, mResVersion);
        }
    }

    public class LocaleText
    {
        public static bool IsCN => LocaleKitEditor.IsCN.Value;
        public static string ResKit => IsCN ? "Res Kit 设置" : "Res Kit Setting";

        public static string GoToPersistent => IsCN ? "打开 Persistent 目录" : "Go To Persistance";

        public static string GenerateClass => IsCN ? "生成代码（资源名常量）" : "Generate Class";

        public static string Build => IsCN ? "打 AB 包" : "Build";

        public static string ForceClear => IsCN ? "清空已生成的 AB" : "ForceClear";

        public static string AutoGenerateClass => IsCN ? "打 AB 包时，自动生成资源名常量代码" : "auto generate class when build";

        public static string SimulationMode =>
            IsCN
                ? "模拟模式（勾选后每当资源修改时无需再打 AB 包，开发阶段建议勾选，打真机包时取消勾选并打一次 AB 包）"
                : "Simulation Mode";

        public static string CancelMark =>
            IsCN
                ? "取消标记"
                : "Cancel Mark";

        public static string Select =>
            IsCN
                ? "选择"
                : "Select";

        public static string MarkedAb =>
            IsCN
                ? "已标记的 AB"
                : "Marked AB";

        // public static string EncryptAB
        // {
        //     get
        //     {
        //         return Language.IsChinese
        //             ? "加密AB(AES加密)"
        //             : "EncryptAB";
        //     }
        // }
        //
        // public static string EncryptKey
        // {
        //     get
        //     {
        //         return Language.IsChinese
        //             ? "加密秘钥(RSA加密)"
        //             : "EncryptKey";
        //     }
        // }
    }
    internal class EditorLifecycle
    {
        static EditorLifecycle()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            ExecuteCommand();
        }
        
        private static Queue<System.Action> mPrivateCommands = new Queue<System.Action>();

        private static Queue<System.Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(System.Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }
}