// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
//
// namespace QFramework.Tests.DOTweenInvalidTween.Editor
// {
//     public static class DOTweenInvalidTweenReproSceneBuilder
//     {
//         public const string ScenePath =
//             "Assets/QFrameworkTests/1.CoreTests/ActionKitTests/DOTweenInvalidTweenRepro/DOTweenInvalidTweenRepro.unity";
//
//         [MenuItem("QFramework/Tests/Create DOTween Invalid Tween Repro Scene")]
//         private static void CreateSceneFromMenu()
//         {
//             if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) CreateSceneBatch();
//         }
//
//         // For -executeMethod; run in a dedicated Editor instance with this project closed elsewhere.
//         public static void CreateSceneBatch()
//         {
//             var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
//             var cameraObject = new GameObject("MainCamera", typeof(Camera));
//             cameraObject.tag = "MainCamera";
//             var camera = cameraObject.GetComponent<Camera>();
//             camera.clearFlags = CameraClearFlags.SolidColor;
//             camera.backgroundColor = new Color(0.05f, 0.07f, 0.10f, 1.0f);
//
//             var root = new GameObject("DOTweenInvalidTweenRepro");
//             root.AddComponent<DOTweenInvalidTweenRepro>();
//             if (!EditorSceneManager.SaveScene(scene, ScenePath))
//             {
//                 throw new System.InvalidOperationException("Could not save reproduction scene: " + ScenePath);
//             }
//
//             AssetDatabase.SaveAssets();
//             Debug.Log("[DOTween Repro] Scene created: " + ScenePath);
//         }
//     }
// }
