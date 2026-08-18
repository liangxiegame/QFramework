/****************************************************************************
 * Copyright (c) 2015 ~ 2026 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public sealed class ArchitectureCodeGeneratorViewState
    {
        internal static readonly ArchitectureCodeType[] SupportedCodeTypes =
        {
            ArchitectureCodeType.Command, ArchitectureCodeType.Query, ArchitectureCodeType.System,
            ArchitectureCodeType.Model, ArchitectureCodeType.Utility, ArchitectureCodeType.Architecture
        };

        private bool mCloseWindowRequested;

        public ArchitectureCodeGeneratorViewState()
        {
            CodeType = ArchitectureCodeType.Architecture;
            InputName = string.Empty;
        }

        public ArchitectureCodeType CodeType { get; set; }
        public string InputName { get; set; }

        public void RestoreCodeType(int persistedCodeType)
        {
            var codeType = (ArchitectureCodeType)persistedCodeType;
            CodeType = Array.IndexOf(SupportedCodeTypes, codeType) < 0
                ? ArchitectureCodeType.Architecture
                : codeType;
        }

        public void LoadCodeTypePreference(string editorPrefsKey)
        {
            RestoreCodeType(EditorPrefs.GetInt(editorPrefsKey, (int)ArchitectureCodeType.Architecture));
        }

        public void SaveCodeTypePreference(string editorPrefsKey)
        {
            RestoreCodeType((int)CodeType);
            EditorPrefs.SetInt(editorPrefsKey, (int)CodeType);
        }

        public void OnGenerationSucceeded()
        {
            InputName = string.Empty;
            mCloseWindowRequested = true;
        }

        public void OnGenerationCompleted(ArchitectureCodeGenerationResult result)
        {
            if (result != null && result.Success) OnGenerationSucceeded();
        }

        public bool ConsumeCloseWindowRequest()
        {
            if (!mCloseWindowRequested) return false;

            mCloseWindowRequested = false;
            return true;
        }
    }

    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(4)]
    [DisplayNameCN("架构代码生成")]
    [DisplayNameEN("Architecture Code Generator")]
    internal class ArchitectureCodeGeneratorView : IPackageKitView
    {
        private const string NamespaceEditorPrefsKeyPrefix = "QF_ARCHITECTURE_CODE_GENERATOR_NAMESPACE";
        private const string OutputRootEditorPrefsKeyPrefix = "QF_ARCHITECTURE_CODE_GENERATOR_OUTPUT_ROOT";
        private const string CodeTypeEditorPrefsKeyPrefix = "QF_ARCHITECTURE_CODE_GENERATOR_CODE_TYPE";
        private const string GenerateInterfaceEditorPrefsKeyPrefix = "QF_ARCHITECTURE_CODE_GENERATOR_GENERATE_INTERFACE";
        private const float CodeTypeListWidth = 140f;
        private const float DividerWidth = 4f;

        private readonly ArchitectureCodeGeneratorViewState mState = new ArchitectureCodeGeneratorViewState();
        private string mNamespace;
        private string mOutputRoot;
        private string mStatusMessage = string.Empty;
        private MessageType mStatusMessageType = MessageType.None;
        private Vector2 mScrollPosition;
        private bool mGenerateInterface;

        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
            mState.LoadCodeTypePreference(GetProjectEditorPrefsKey(CodeTypeEditorPrefsKeyPrefix));
            mNamespace = EditorPrefs.GetString(GetProjectEditorPrefsKey(NamespaceEditorPrefsKeyPrefix),
                GetProjectNamespace());
            mOutputRoot = EditorPrefs.GetString(GetProjectEditorPrefsKey(OutputRootEditorPrefsKeyPrefix),
                "Assets/Scripts");
            mGenerateInterface = EditorPrefs.GetBool(
                GetProjectEditorPrefsKey(GenerateInterfaceEditorPrefsKeyPrefix), false);
        }

        public void OnGUI()
        {
            GUILayout.Space(8);
            GUILayout.Label(LocaleText.Title, Styles.Title);
            GUILayout.Space(8);

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical(GUILayout.Width(CodeTypeListWidth), GUILayout.ExpandHeight(true));
            DrawCodeTypeList();
            GUILayout.EndVertical();

            GUILayout.Box(GUIContent.none, Styles.Divider,
                GUILayout.Width(DividerWidth), GUILayout.ExpandHeight(true));

            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawFormAndPreview();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawCodeTypeList()
        {
            GUILayout.Space(4);
            GUILayout.Label(LocaleText.CodeType, Styles.Label);
            GUILayout.Space(4);

            foreach (var codeType in ArchitectureCodeGeneratorViewState.SupportedCodeTypes)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Space(8);
                GUILayout.Label(codeType.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                var rect = GUILayoutUtility.GetLastRect();
                if (mState.CodeType == codeType)
                    GUI.Box(rect, "", Styles.SelectionRect);

                if (rect.Contains(Event.current.mousePosition) &&
                    Event.current.type == EventType.MouseUp)
                {
                    HandleCodeTypeSelection(codeType);
                    Event.current.Use();
                }
            }
        }

        private void DrawFormAndPreview()
        {
            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.BeginVertical("box");
            {
                mState.InputName = EditorGUILayout.TextField(LocaleText.Name, mState.InputName);
                EditorGUILayout.HelpBox(GetNameHint(), MessageType.Info);

                mNamespace = EditorGUILayout.TextField(LocaleText.Namespace, mNamespace);

                GUILayout.BeginHorizontal();
                mOutputRoot = EditorGUILayout.TextField(LocaleText.OutputRoot, mOutputRoot);
                if (GUILayout.Button(LocaleText.SelectFolder, GUILayout.Width(80))) SelectOutputRoot();
                GUILayout.EndHorizontal();

                if (ArchitectureCodeGenerator.SupportsInterfaceGeneration(mState.CodeType))
                {
                    mGenerateInterface = EditorGUILayout.Toggle(LocaleText.GenerateInterface, mGenerateInterface);
                    if (mGenerateInterface)
                    {
                        var registerMethod = ArchitectureCodeGenerator.GetRegisterMethodName(mState.CodeType);
                        EditorGUILayout.HelpBox(LocaleText.InterfaceRegistrationHint(registerMethod), MessageType.Info);
                    }
                }
            }
            GUILayout.EndVertical();

            var preview = ArchitectureCodeGenerator.CreatePreview(mState.CodeType, mState.InputName, mNamespace,
                mOutputRoot, mGenerateInterface);
            var fileExists = preview.IsValid && File.Exists(preview.AssetPath);

            GUILayout.Space(8);
            GUILayout.Label(LocaleText.Preview, Styles.SectionTitle);
            GUILayout.BeginVertical("box");
            {
                DrawPreviewValue(LocaleText.ClassName, preview.ClassName);
                DrawPreviewValue(LocaleText.Namespace, preview.Namespace);
                DrawPreviewValue(LocaleText.AssetPath, preview.AssetPath);

                GUILayout.Space(6);
                GUILayout.Label(LocaleText.Code, Styles.Label);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextArea(preview.Code, GUILayout.MinHeight(220));
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndVertical();

            if (!preview.IsValid)
                EditorGUILayout.HelpBox(preview.ErrorMessage, MessageType.Warning);
            else if (fileExists)
                EditorGUILayout.HelpBox(LocaleText.FileExists + preview.AssetPath, MessageType.Error);

            if (!string.IsNullOrEmpty(mStatusMessage))
                EditorGUILayout.HelpBox(mStatusMessage, mStatusMessageType);

            EditorGUI.BeginDisabledGroup(!preview.IsValid || fileExists);
            if (GUILayout.Button(LocaleText.Create, GUILayout.Height(32))) Generate(preview);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
        }

        public void OnShow()
        {
            mStatusMessage = string.Empty;
        }

        public void OnUpdate()
        {
        }

        public void OnHide()
        {
            SavePreferences();
        }

        public void OnWindowGUIEnd()
        {
            if (!mState.ConsumeCloseWindowRequest()) return;

            var window = EditorWindow;
            RenderEndCommandExecutor.PushCommand(() =>
            {
                if (window) window.Close();
            });
        }

        public void OnDispose()
        {
            SavePreferences();
        }

        private void Generate(ArchitectureCodeGenerationPreview preview)
        {
            SavePreferences();

            var result = ArchitectureCodeGenerator.Generate(preview);
            if (!result.Success)
            {
                mState.OnGenerationCompleted(result);
                mStatusMessage = result.ErrorMessage;
                mStatusMessageType = MessageType.Error;
                return;
            }

            AssetDatabase.ImportAsset(result.AssetPath, ImportAssetOptions.ForceSynchronousImport);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(result.AssetPath);
            if (script)
            {
                Selection.activeObject = script;
                EditorGUIUtility.PingObject(script);
                AssetDatabase.OpenAsset(script);
            }

            mState.OnGenerationCompleted(result);
            mStatusMessage = LocaleText.Created + result.AssetPath;
            mStatusMessageType = MessageType.Info;

            if (mGenerateInterface && ArchitectureCodeGenerator.SupportsInterfaceGeneration(preview.CodeType))
            {
                var registerMethod = ArchitectureCodeGenerator.GetRegisterMethodName(preview.CodeType);
                Debug.Log(LocaleText.InterfaceRegisteredLogHint(registerMethod));
            }
        }

        private void SelectOutputRoot()
        {
            var currentAbsolutePath = Path.GetFullPath(string.IsNullOrEmpty(mOutputRoot) ? "Assets" : mOutputRoot);
            if (!Directory.Exists(currentAbsolutePath)) currentAbsolutePath = Application.dataPath;

            var selectedPath = EditorUtility.OpenFolderPanel(LocaleText.SelectFolder, currentAbsolutePath,
                string.Empty);
            if (string.IsNullOrEmpty(selectedPath)) return;

            var normalizedSelectedPath = selectedPath.Replace('\\', '/').TrimEnd('/');
            var normalizedAssetsPath = Application.dataPath.Replace('\\', '/').TrimEnd('/');

            if (normalizedSelectedPath == normalizedAssetsPath)
            {
                mOutputRoot = "Assets";
                mStatusMessage = string.Empty;
            }
            else if (normalizedSelectedPath.StartsWith(normalizedAssetsPath + "/", StringComparison.Ordinal))
            {
                mOutputRoot = "Assets" + normalizedSelectedPath.Substring(normalizedAssetsPath.Length);
                mStatusMessage = string.Empty;
            }
            else
            {
                mStatusMessage = LocaleText.MustBeInsideAssets;
                mStatusMessageType = MessageType.Error;
            }
        }

        private void SavePreferences()
        {
            SaveCodeTypePreference();
            EditorPrefs.SetString(GetProjectEditorPrefsKey(NamespaceEditorPrefsKeyPrefix),
                (mNamespace ?? string.Empty).Trim());
            EditorPrefs.SetString(GetProjectEditorPrefsKey(OutputRootEditorPrefsKeyPrefix),
                (mOutputRoot ?? string.Empty).Trim());
            EditorPrefs.SetBool(GetProjectEditorPrefsKey(GenerateInterfaceEditorPrefsKeyPrefix),
                mGenerateInterface);
        }

        private void SaveCodeTypePreference()
        {
            mState.SaveCodeTypePreference(GetProjectEditorPrefsKey(CodeTypeEditorPrefsKeyPrefix));
        }

        private void HandleCodeTypeSelection(ArchitectureCodeType codeType)
        {
            if (mState.CodeType == codeType) return;
            mState.CodeType = codeType;
            SaveCodeTypePreference();
        }

        private static string GetProjectEditorPrefsKey(string prefix)
        {
            return prefix + ":" + Application.dataPath.Replace('\\', '/');
        }

        private static string GetProjectNamespace()
        {
            var existingNamespaces = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/Scripts" }))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                var scriptClass = script ? script.GetClass() : null;
                if (scriptClass != null && !string.IsNullOrEmpty(scriptClass.Namespace))
                    existingNamespaces.Add(scriptClass.Namespace);
            }

            var setting = CodeGenKit.Setting;
            return ArchitectureCodeGenerator.ResolveDefaultNamespace(setting.Namespace, setting.IsDefaultNamespace,
                existingNamespaces, PlayerSettings.productName);
        }

        private string GetNameHint()
        {
            if (mState.CodeType == ArchitectureCodeType.Architecture) return LocaleText.ArchitectureNameHint;
            return string.Format(LocaleText.SuffixNameHint, mState.CodeType.ToString());
        }

        private static void DrawPreviewValue(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, Styles.Label, GUILayout.Width(90));
            GUILayout.Label(string.IsNullOrEmpty(value) ? "-" : value, EditorStyles.label);
            GUILayout.EndHorizontal();
        }

        private static class Styles
        {
            public static readonly GUIStyle SelectionRect = "SelectionRect";
            public static readonly GUIStyle Divider = new GUIStyle("box")
            {
                margin = new RectOffset(0, 0, 0, 0)
            };

            public static readonly GUIStyle Title = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };

            public static readonly GUIStyle SectionTitle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };

            public static readonly GUIStyle Label = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
        }

        private static class LocaleText
        {
            private static bool IsCN => LocaleKitEditor.IsCN.Value;

            public static string Title => IsCN ? "QFramework 架构代码生成" : "QFramework Architecture Code Generator";
            public static string CodeType => IsCN ? "代码类型" : "Code Type";
            public static string Name => IsCN ? "名字" : "Name";
            public static string Namespace => IsCN ? "命名空间" : "Namespace";
            public static string OutputRoot => IsCN ? "生成根目录" : "Output Root";
            public static string SelectFolder => IsCN ? "选择" : "Browse";
            public static string Preview => IsCN ? "预览" : "Preview";
            public static string ClassName => IsCN ? "类名" : "Class Name";
            public static string AssetPath => IsCN ? "生成路径" : "Asset Path";
            public static string Code => IsCN ? "代码" : "Code";
            public static string Create => IsCN ? "创建" : "Create";
            public static string FileExists => IsCN ? "文件已存在，不会覆盖：" : "File already exists and will not be overwritten: ";
            public static string Created => IsCN ? "已创建：" : "Created: ";
            public static string MustBeInsideAssets => IsCN ? "生成目录必须位于当前项目的 Assets 目录中" : "The output folder must be inside this project's Assets folder.";
            public static string ArchitectureNameHint => IsCN ? "Architecture 使用完整类名，例如 CounterApp。" : "Architecture uses the complete class name, for example CounterApp.";
            public static string SuffixNameHint => IsCN ? "可以输入基础名；将自动补全 {0} 后缀。" : "Enter a base name; the {0} suffix is added automatically.";
            public static string GenerateInterface => IsCN ? "生成接口" : "Generate Interface";
            public static string InterfaceRegistrationHint(string registerMethod) => IsCN
                ? "勾选后会同时生成接口（继承 ISystem/IModel/IUtility）并由类实现；请在 Architecture.Init() 中用接口类型注册，例如：this." + registerMethod + "<IXxx…>(new Xxx…())"
                : "Generates an interface (extending ISystem/IModel/IUtility) that the class implements; register it by interface in Architecture.Init(), e.g. this." + registerMethod + "<IXxx…>(new Xxx…())";
            public static string InterfaceRegisteredLogHint(string registerMethod) => IsCN
                ? "[QFramework] 已生成模块接口。请在 Architecture.Init() 中用接口类型注册，例如 this." + registerMethod + "<IXxx…>(new Xxx…()); 否则按接口获取会返回 null。"
                : "[QFramework] Module interface generated. Register it by interface in Architecture.Init(), e.g. this." + registerMethod + "<IXxx…>(new Xxx…()); otherwise fetching by interface returns null.";
        }
    }
}
#endif
