using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class ResKitView
    {
        private string mResVersion = "100";

        private bool mEnableGenerateClass
        {
            get => EditorPrefs.GetBool(KEY_AUTOGENERATE_CLASS, false);
            set => EditorPrefs.SetBool(KEY_AUTOGENERATE_CLASS, value);
        }

        private static bool mAppendHash
        {
            get => EditorPrefs.GetBool(KEY_APPEND_HASH, false);
            set => EditorPrefs.SetBool(KEY_APPEND_HASH, value);
        }

        public static bool AppendHash => mAppendHash;

        public static int GenerateClassNameStyle
        {
            get => EditorPrefs.GetInt(KEY_GENERATE_CLASS_NAME_STYLE, 0);
            set => EditorPrefs.SetInt(KEY_GENERATE_CLASS_NAME_STYLE, value);
        }

        private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
        public const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";
        public const string KEY_APPEND_HASH = "KEY_APPEND_HASH";
        public const string KEY_GENERATE_CLASS_NAME_STYLE = "KEY_GENERATE_CLASS_NAME_STYLE";


        public void Init()
        {
            mResVersion = EditorPrefs.GetString(KEY_QAssetBundleBuilder_RESVERSION, "100");
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

            GUILayout.BeginHorizontal("box");
            GUILayout.Label(LocaleText.TargetPlatform, GUILayout.Width(100));
            GUILayout.Label(AssetBundlePathHelper.GetPlatformName());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("PresistentPath:", GUILayout.Width(100));
            GUILayout.Label(Application.persistentDataPath);
            if (GUILayout.Button(LocaleText.Open, GUILayout.MaxWidth(100)))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            mEnableGenerateClass = GUILayout.Toggle(mEnableGenerateClass, LocaleText.AutoGenerateClass);

            if (mEnableGenerateClass)
            {
                GUILayout.FlexibleSpace();
                GenerateClassNameStyle =
                    EditorGUILayout.Popup(GenerateClassNameStyle, LocaleText.GenerateClassNameStyleItems);
            }

            GUILayout.EndHorizontal();
            mAppendHash = GUILayout.Toggle(mAppendHash, LocaleText.AppendHash);
            GUILayout.BeginHorizontal();
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

            public static string TargetPlatform => IsCN ? "目标平台: " : "Target Platform: ";
            public static string Open => IsCN ? "打开" : "Open";

            public static string GenerateClass => IsCN ? "生成代码（资源名常量）" : "Generate Class";

            public static string Build => IsCN ? "打 AB 包" : "Build";

            public static string ForceClear => IsCN ? "清空已生成的 AB" : "ForceClear";

            public static string AutoGenerateClass => IsCN ? "打 AB 包时，自动生成资源名常量代码" : "auto generate class when build";

            public static string AppendHash => IsCN ? "打 AB 包时，在后缀增加 hash (微信小游戏需要)" : "append hash when build";
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
}