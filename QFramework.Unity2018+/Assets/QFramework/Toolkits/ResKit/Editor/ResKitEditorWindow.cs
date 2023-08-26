/****************************************************************************
 * Copyright (c) 2016 ~ 2023 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;

namespace QFramework
{
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

        private bool mEnableGenerateClass
        {
            get => EditorPrefs.GetBool(KEY_AUTOGENERATE_CLASS, false);
            set => EditorPrefs.SetBool(KEY_AUTOGENERATE_CLASS, value);
        }

        private int mBuildTargetIndex = 0;

        public static int GenerateClassNameStyle
        {
            get => EditorPrefs.GetInt(KEY_GENERATE_CLASS_NAME_STYLE, 0);
            set => EditorPrefs.SetInt(KEY_GENERATE_CLASS_NAME_STYLE, value);
        }

        private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
        public const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";
        public const string KEY_GENERATE_CLASS_NAME_STYLE = "KEY_GENERATE_CLASS_NAME_STYLE";


        public void Init()
        {
            mResVersion = EditorPrefs.GetString(KEY_QAssetBundleBuilder_RESVERSION, "100");

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


        public const int GENERATE_NAME_STYLE_UPPERCASE = 0;
        public const int GENERATE_NAME_STYLE_KeepOriginal = 1;

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

            GUILayout.BeginHorizontal();
            mEnableGenerateClass = GUILayout.Toggle(mEnableGenerateClass, LocaleText.AutoGenerateClass);

            if (mEnableGenerateClass)
            {
                GUILayout.FlexibleSpace();
                GenerateClassNameStyle =
                    EditorGUILayout.Popup(GenerateClassNameStyle, LocaleText.GenerateClassNameStyleItems);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var index = ResKitEditorAPI.SimulationMode ? 0 : 1;
            index = EditorGUILayout.Popup(index, LocaleText.ModeMenu, GUILayout.Width(100));
            if (index != (ResKitEditorAPI.SimulationMode ? 0 : 1))
            {
                ResKitEditorAPI.SimulationMode = (index == 0);
            }

            if (ResKitEditorAPI.SimulationMode)
            {
                GUILayout.Label(LocaleText.SimulationModeDescription);
            }
            else
            {
                GUILayout.Label(LocaleText.DeviceModeDescription);
            }

            GUILayout.EndHorizontal();

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
            EditorPrefs.SetString(KEY_QAssetBundleBuilder_RESVERSION, mResVersion);
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

            private static string[] mGenerateClassNameStyleItemsCN = new string[]
            {
                "全大写（UILoginPanel=>UILOGINPANEL）",
                "保持原名（UILoginPanel=>UILoginPanel）"
            };

            private static string[] mGenerateClassNameStyleItemsEN = new[]
            {
                "UPPERCASE(UILoginPanel=>UILOGINPANEL)",
                "KeepOriginal(UILoginPanel=>UILoginPanel)"
            };

            public static string[] GenerateClassNameStyleItems =>
                IsCN ? mGenerateClassNameStyleItemsCN : mGenerateClassNameStyleItemsEN;

            private static string[] mModeMenuCN = new[]
            {
                "模拟模式",
                "真机模式"
            };

            private static string[] mModeMenuEN = new[]
            {
                "SimulationMode",
                "DeviceMode"
            };

            public static string[] ModeMenu => IsCN ? mModeMenuCN : mModeMenuEN;

            public static string SimulationModeDescription =>
                IsCN
                    ? "不用主动调用 ResKit.Init 或 ResKit.InitAsync.每当资源修改时无需再打 AB 包，开发阶段建议选择"
                    : "Don't need to call ResKit.Init or ResKit.InitAsync.When Assets modified, don't need to build AB package";

            public static string DeviceModeDescription =>
                IsCN
                    ? "每当资源修改时需要打 AB 包，需要在游戏运行后调用 ResKit.Init 或 ResKit.InitAsync，在真机上只支持此模式"
                    : "Need call ResKit.Init or ResKit.InitAsync after game run.When Assets modified, need to build AB.Only this mode is supported on real machines";

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