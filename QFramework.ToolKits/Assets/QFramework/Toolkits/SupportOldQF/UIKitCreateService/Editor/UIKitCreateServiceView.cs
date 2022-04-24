using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class UIKitCreateServiceView
    {
        public EditorWindow EditorWindow { get; set; }
        public void Init()
        {
        }

        private string mPanelNameToCreate = "UIHomePanel";

        public void OnGUI()
        {
            mPanelNameToCreate = EditorGUILayout.TextField(mPanelNameToCreate);

            if (GUILayout.Button(LocaleText.CreateUIPanel))
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
                var fullScenePath = "Assets/Scenes/TestUIPanels/".CreateDirIfNotExists()
                    .Builder()
                    .Append("Test{0}.unity".FillFormat(panelName)).ToString();

                var uiKitSettingData = UIKitSettingData.Load();
                
                var panelPrefabPath = $"Assets{uiKitSettingData.UIPrefabDir}/"
                    .CreateDirIfNotExists()
                    .Builder()
                    .Append("{0}.prefab".FillFormat(panelName)).ToString();

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

                    var uirootPrefab = Resources.Load<GameObject>("UIRoot");
                    var uiroot = Object.Instantiate(uirootPrefab);
                    uiroot.name = "UIRoot";

                    var designTransform = uiroot.transform.Find("Design");

                    var gameObj = new GameObject(panelName);
                    gameObj.transform.parent = designTransform;
                    gameObj.transform.localScale = Vector3.one;

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
        }

    }
}
