using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    public class UIKitSettingViewModel: IController,ICanGetUtility
    {

        private string mPanelNameToCreate = "UIHomePanel";

        public string PanelNameToCreate
        {
            get { return mPanelNameToCreate; }
            set { mPanelNameToCreate = value; }
        }
        
        public void OnCreateUIPanelClick()
        {

            var panelName = mPanelNameToCreate;

            if (panelName.IsNotNullAndEmpty())
            {
                var fullScenePath = "Assets/Scenes/TestUIPanels/".CreateDirIfNotExists()
                    .Append("Test{0}.unity".FillFormat(panelName)).ToString();

                var panelPrefabPath = "Assets/Art/UIPrefab/".CreateDirIfNotExists()
                    .Append("{0}.prefab".FillFormat(panelName)).ToString();

                if (File.Exists(panelPrefabPath))
                {
                    this.GetUtility<IEditorDialogUtility>()
                        .ShowErrorMsg("UI 界面已存在:{0}".FillFormat(panelPrefabPath));
                    return;
                }
                
                if (File.Exists(fullScenePath))
                {
                    this.GetUtility<IEditorDialogUtility>()
                        .ShowErrorMsg("测试场景已存在:{0}".FillFormat(fullScenePath));
                    return;
                }
                
                RenderEndCommandExecutor.PushCommand(() =>
                {
                    var currentScene = SceneManager.GetActiveScene();
                    EditorSceneManager.SaveScene(currentScene);

                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                    EditorSceneManager.SaveScene(scene, fullScenePath);

                    var uiroot = Resources.Load<GameObject>("UIRoot").Instantiate().Name("UIRoot");

                    var designTransform = uiroot.transform.Find("Design");

                    var gameObj = new GameObject(panelName);
                    gameObj.transform.Parent(designTransform)
                        .LocalScaleIdentity();

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
                    // var uiPanelTester = tester.AddComponent<UIPanelTester>();
                    // uiPanelTester.PanelName = panelName;

                    // 开始生成代码
                    UICodeGenerator.DoCreateCode(new[] {prefab});
                });
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PackageKit.Interface;
        }
    }
}