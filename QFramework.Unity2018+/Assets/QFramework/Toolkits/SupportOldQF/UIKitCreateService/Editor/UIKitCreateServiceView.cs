using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class UIKitCreateServiceView
    {
        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
        }

        private string mPanelNameToCreate = string.Empty;

        private string mModuleFolder
        {
            get => EditorPrefs.GetString("UIKitCreate" + nameof(mModuleFolder), "Assets");
            set => EditorPrefs.SetString("UIKitCreate" + nameof(mModuleFolder), value);
        }

        private int mResolutionWidth
        {
            get => EditorPrefs.GetInt("UIKitCreate" + nameof(mResolutionWidth), 1280);
            set => EditorPrefs.SetInt("UIKitCreate" + nameof(mResolutionWidth), value);
        }

        private int mResolutionHeight
        {
            get => EditorPrefs.GetInt("UIKitCreate" + nameof(mResolutionHeight), 720);
            set => EditorPrefs.SetInt("UIKitCreate" + nameof(mResolutionHeight), value);
        }

        private float mResolutionMatchScreenOrHeight
        {
            get => EditorPrefs.GetFloat("UIKitCreate" + nameof(mResolutionMatchScreenOrHeight), 0);
            set => EditorPrefs.SetFloat("UIKitCreate" + nameof(mResolutionMatchScreenOrHeight), value);
        }

        private string mPrefabGeneratePath =>
            $"{mModuleFolder}{UIKitSettingData.Load().UIPrefabDir}/{mPanelNameToCreate}.prefab";

        private string mSceneGeneratePath =>
            $"{mModuleFolder}/Scenes/TestUIPanels/{mPanelNameToCreate}".GetFolderPath().Replace("\\","/") +
            $"/Test{mPanelNameToCreate.GetFileNameWithoutExtend()}.unity";

        private string mMainCodeFileGenreatePath => mPrefabGeneratePath.Replace(UIKitSettingData.Load().UIPrefabDir,
                UIKitSettingData.Load().UIScriptDir)
            .Replace(".prefab", ".cs");

        private string mDesignerCodeFileGenreatePath => mPrefabGeneratePath.Replace(UIKitSettingData.Load().UIPrefabDir,
                UIKitSettingData.Load().UIScriptDir)
            .Replace(".prefab", ".Designer.cs");


        private Lazy<GUIStyle> mLabelStyle = new Lazy<GUIStyle>(() =>
        {
            var labelStyle = new GUIStyle(GUI.skin.GetStyle("label"))
            {
                richText = true
            };
            return labelStyle;
        });

        public void OnGUI()
        {
            EditorGUILayout.LabelField(LocaleText.ResolutionOrMatchWidthOrHeight);

            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            mResolutionWidth = EditorGUILayout.IntField(mResolutionWidth, GUILayout.Width(50));
            EditorGUILayout.LabelField("x", GUILayout.Width(10));
            mResolutionHeight = EditorGUILayout.IntField(mResolutionHeight, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            mResolutionMatchScreenOrHeight = EditorGUILayout.Slider("MatchWidthOrHeight",
                mResolutionMatchScreenOrHeight, 0, 1, GUILayout.Width(300));
            GUILayout.EndVertical();

            GUILayout.EndVertical();

            EditorGUILayout.LabelField(LocaleText.ModuleFolder);
            GUILayout.BeginHorizontal("box");

            EditorGUILayout.LabelField(mModuleFolder);
            if (GUILayout.Button("..."))
            {
                var folderPath = EditorUtility.OpenFolderPanel(LocaleText.ModuleFolder, mModuleFolder, "Assets");

                mModuleFolder = folderPath.RemoveString(Application.dataPath).Builder().AddPrefix("Assets").ToString();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField(LocaleText.PanelName);

            GUILayout.BeginHorizontal("box");
            mPanelNameToCreate = EditorGUILayout.TextField(mPanelNameToCreate);
            GUILayout.EndHorizontal();

            if (mPanelNameToCreate.IsNotNullAndEmpty())
            {
                EditorGUILayout.LabelField(LocaleText.Preview);
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(
                    File.Exists(mPrefabGeneratePath)
                        ? mPrefabGeneratePath + " " + LocaleText.AlreadyExists
                        : mPrefabGeneratePath, mLabelStyle.Value);
                EditorGUILayout.LabelField(
                    File.Exists(mSceneGeneratePath)
                        ? mSceneGeneratePath + " " + LocaleText.AlreadyExists
                        : mSceneGeneratePath, mLabelStyle.Value);
                EditorGUILayout.LabelField(
                    File.Exists(mMainCodeFileGenreatePath)
                        ? mMainCodeFileGenreatePath + " " + LocaleText.AlreadyExists
                        : mMainCodeFileGenreatePath, mLabelStyle.Value);
                EditorGUILayout.LabelField(
                    File.Exists(mDesignerCodeFileGenreatePath)
                        ? mDesignerCodeFileGenreatePath + " " + LocaleText.AlreadyExists
                        : mDesignerCodeFileGenreatePath, mLabelStyle.Value);
                GUILayout.EndVertical();
            }

            if (mPanelNameToCreate.IsNotNullAndEmpty() && GUILayout.Button(LocaleText.CreateUIPanel))
            {
                OnCreateUIPanelClick();

                GUIUtility.ExitGUI();
            }
        }

        private Queue<Action> mCommands = new Queue<Action>();

        public void OnWindowGUIEnd()
        {
            if (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }

        public void OnDestroy()
        {
        }


        public void OnCreateUIPanelClick()
        {
            var panelName = mPanelNameToCreate;

            if (!string.IsNullOrEmpty(panelName))
            {
                var fullScenePath = $"{mModuleFolder}/Scenes/TestUIPanels/{panelName}".GetFolderPath()
                    .CreateDirIfNotExists()
                    .Builder()
                    .Append("/Test{0}.unity".FillFormat(panelName.GetFileNameWithoutExtend()))
                    .ToString();

                var uiKitSettingData = UIKitSettingData.Load();

                var panelPrefabPath = $"{mModuleFolder}{uiKitSettingData.UIPrefabDir}/{panelName}"
                    .GetFolderPath()
                    .CreateDirIfNotExists()
                    .Builder()
                    .Append("/{0}.prefab".FillFormat(panelName.GetFileNameWithoutExtend())).ToString();

                if (File.Exists(panelPrefabPath))
                {
                    EditorUtility.DisplayDialog("error", "UI 界面已存在:{0}".FillFormat(panelPrefabPath), "OK");

                    return;
                }

                if (File.Exists(fullScenePath))
                {
                    EditorUtility.DisplayDialog("error", "测试场景已存在:{0}".FillFormat(fullScenePath), "OK");

                    return;
                }


                mCommands.Enqueue(() =>
                {
                    if (EditorWindow)
                    {
                        EditorWindow.Close();
                    }

                    var currentScene = SceneManager.GetActiveScene();
                    EditorSceneManager.SaveScene(currentScene);

                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                    EditorSceneManager.SaveScene(scene, fullScenePath);

                    var camera = new GameObject("Camera").AddComponent<Camera>();
                    camera.clearFlags = CameraClearFlags.Color;
                    camera.backgroundColor = Color.black;

                    var uirootPrefab = Resources.Load<GameObject>("UIRoot");
                    var uiroot = Object.Instantiate(uirootPrefab);
                    uiroot.name = "UIRoot";
                    uiroot.GetComponent<CanvasScaler>()
                        .Self(scaler =>
                        {
                            scaler.referenceResolution = new Vector2(mResolutionWidth, mResolutionHeight);
                            scaler.matchWidthOrHeight = mResolutionMatchScreenOrHeight;
                        });

                    var designTransform = uiroot.transform.Find("Design");

                    var gameObj = new GameObject(panelName.GetFileNameWithoutExtend())
                    {
                        transform =
                        {
                            parent = designTransform,
                            localScale = Vector3.one
                        }
                    };

                    var rectTransform = gameObj.AddComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;

                    var prefab = PrefabUtils.SaveAndConnect(panelPrefabPath, gameObj);

                    EditorSceneManager.SaveScene(scene);

                    // 标记 AssetBundle
                    ResKitAssetsMenu.MarkAB(panelPrefabPath);
                    
                    var tester = new GameObject("Test{0}".FillFormat(panelName));
                    var uiPanelTester = tester.AddComponent<ResKitUIPanelTester>();
                    uiPanelTester.PanelName = panelName;

                    // 开始生成代码
                    UICodeGenerator.DoCreateCode(new[] { prefab });
                });
            }
        }

        class LocaleText
        {
            public static string CreateUIPanel => LocaleKitEditor.IsCN.Value ? "创建 UI Panel" : "Create UI Panel";
            public static string ModuleFolder => LocaleKitEditor.IsCN.Value ? "模块目录:" : "Module Folder";

            public static string ResolutionOrMatchWidthOrHeight =>
                LocaleKitEditor.IsCN.Value ? "分辨率&对齐:" : "Resolution&MatchWidthOrHeight:";

            public static string PanelName => LocaleKitEditor.IsCN.Value
                ? "界面名字:(例如 UIHomePanel、Game/UIGamePanel)"
                : "Panel Name:(UIHomePanel、Game/UIGamePanel etc)";

            public static string Preview => LocaleKitEditor.IsCN.Value ? "生成文件预览:" : "Preview:";

            public static string AlreadyExists => LocaleKitEditor.IsCN.Value
                ? "<color=red>[已存在]</color>"
                : "<color=red>[Exists]</color>";
        }
    }
}