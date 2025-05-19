namespace QFramework
{
    public interface IBuildAction : IBuildView
    {
        void Build();
    }
    
// #if UNITY_EDITOR
// using System.IO;
// using ICSharpCode.SharpZipLib.Zip;
// using QFramework;
// using UnityEditor;
// using UnityEngine;
//
// namespace HiEggplant2
// {
//     [PackageKitGroup("鬼山之下")]
//     [DisplayNameCN("打包管理")]
//     public class BuildEditor : IPackageKitView
//     {
//         public EditorWindow EditorWindow { get; set; }
//
//         private string filePath => "Assets/Scripts/Config/Game/Resources".CreateDirIfNotExists()
//             .Builder().Append("/GameConfig.json").ToString();
//
//
//         private GameConfig mConfig;
//         
//         public class GameConfig
//         {
//             
//             public enum PublishModes
//             {
//                 Development,
//                 DemoDev,
//                 SteamDemo,
//                 PlayTest,
//                 Steam
//             }
//             
//             public PublishModes PublishMode;
//             public bool Debug;
//             public string Version;
//         }
//         
//
//
//         public void Init()
//         {
//         }
//
//         public void OnShow()
//         {
//             if (File.Exists(filePath))
//             {
//                 var content = File.ReadAllText(filePath);
//                 mConfig = JsonUtility.FromJson<GameConfig>(content);
//                 if (mConfig == null)
//                 {
//                     mConfig = new GameConfig();
//                 }
//             }
//             else
//             {
//                 mConfig = new GameConfig();
//             }
//         }
//
//         void Save()
//         {
//             File.WriteAllText(filePath, JsonUtility.ToJson(mConfig, true));
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//         }
//
//         public void OnGUI()
//         {
//             EditorGUI.BeginChangeCheck();
//
//             mConfig.PublishMode = (GameConfig.PublishModes)EditorGUILayout.EnumPopup(mConfig.PublishMode);
//             mConfig.Debug = GUILayout.Toggle(mConfig.Debug, "调试");
//             if (mConfig.Debug)
//             {
//                 // mConfig.DebugGrassBattle = GUILayout.Toggle(mConfig.DebugGrassBattle, "除草测试");
//                 // mConfig.DebugBugBattle = GUILayout.Toggle(mConfig.DebugBugBattle, "除虫测试");
//                 // mConfig.DebugSoilGodSprite = GUILayout.Toggle(mConfig.DebugSoilGodSprite, "土地公测试");
//                 // mConfig.DebugWellnessSprite = GUILayout.Toggle(mConfig.DebugWellnessSprite, "养生茄子测试");
//                 // mConfig.DebugSlotSystem = GUILayout.Toggle(mConfig.DebugSlotSystem, "背包系统测试");
//             }
//
//             mConfig.Version = EditorGUILayout.TextField(mConfig.Version);
//
//             GUILayout.Space(10);
//             GUILayout.BeginHorizontal();
//             GUILayout.Label("版本:");
//             if (EditorGUI.EndChangeCheck())
//             {
//                 Save();
//             }
//
//             GUILayout.EndHorizontal();
//             GUILayout.Space(10);
//
//             if (GUILayout.Button("打包开发版"))
//             {
//                 // mConfig.Debug = false;
//                 mConfig.PublishMode = GameConfig.PublishModes.Development;
//                 Save();
//                 ResKitEditorAPI.BuildAssetBundles();
//                 PlayerSettings.bundleVersion = mConfig.Version;
//                 PlayerSettings.productName = "HiEggplant2";
//                 var scenes = EditorBuildSettings.scenes;
//                 var outputFile = Application.dataPath + "/../Builds/HiEggplant2Window/HiEggplant2.exe";
//                 var target = BuildTarget.StandaloneWindows64;
//                 var options = BuildOptions.None;
//                 EditorUserBuildSettings.development = false;
//                 BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
//                 EditorUtility.RevealInFinder(outputFile);
//             }
//
//             if (GUILayout.Button("打包 Demo 开发版"))
//             {
//                 mConfig.Debug = true;
//                 mConfig.PublishMode = GameConfig.PublishModes.DemoDev;
//                 Save();
//                 PlayerSettings.productName = "HiEggplant2DemoDev";
//                 ResKitEditorAPI.BuildAssetBundles();
//                 PlayerSettings.bundleVersion = mConfig.Version;
//                 var scenes = EditorBuildSettings.scenes;
//                 var outputFile = Application.dataPath + "/../Builds/HiEggplant2DemoDev/HiEggplant2DemoDev.exe";
//                 var target = BuildTarget.StandaloneWindows64;
//                 var options = BuildOptions.Development;
//                 EditorUserBuildSettings.development = true;
//                 outputFile.GetFolderPath().DeleteDirIfExists();
//                 BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
//                 EditorUtility.RevealInFinder(outputFile.GetFolderPath());
//             }
//
//             if (GUILayout.Button("打包 Steam Demo 版"))
//             {
//                 mConfig.PublishMode = GameConfig.PublishModes.SteamDemo;
//                 Save();
//                 PlayerSettings.productName = "HiEggplant2Demo";
//                 ResKitEditorAPI.BuildAssetBundles();
//                 PlayerSettings.bundleVersion = mConfig.Version;
//
//                 var scenes = EditorBuildSettings.scenes;
//                 var outputFile = Application.dataPath + "/../Builds/HiEggplant2SteamDemo/HiEggplant2Demo.exe";
//                 var zipFileFolder = Application.dataPath + "/../Builds/";
//                 var target = BuildTarget.StandaloneWindows64;
//                 var options = BuildOptions.None;
//                 EditorUserBuildSettings.development = false;
//                 outputFile.GetFolderPath().DeleteDirIfExists();
//                 BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
//                 var fastZip = new FastZip();
//                 fastZip.CreateZip(zipFileFolder + "HiEggplant2Demo_" + mConfig.Version + ".zip",
//                     outputFile.GetFolderPath(), true, string.Empty);
//                 
//                 var moveFilePath = "D:\\我的坚果云\\" + "HiEggplant2Demo_" + mConfig.Version + ".zip";
//                 File.Move(zipFileFolder + "HiEggplant2Demo_" + mConfig.Version + ".zip",moveFilePath);
//                 EditorUtility.RevealInFinder("D:\\我的坚果云\\");
//             }
//
//             if (GUILayout.Button("打包 Steam PlayTest 版"))
//             {
//                 // mConfig.Debug = false;
//                 mConfig.PublishMode = GameConfig.PublishModes.PlayTest;
//                 Save();
//
//                 ResKitEditorAPI.BuildAssetBundles();
//                 PlayerSettings.bundleVersion = mConfig.Version;
//                 PlayerSettings.productName = "HiEggplant2";
//                 var scenes = EditorBuildSettings.scenes;
//                 var outputFile = Application.dataPath + "/../Builds/HiEggplant2Window/HiEggplant2.exe";
//                 var target = BuildTarget.StandaloneWindows64;
//                 var options = BuildOptions.Development;
//                 EditorUserBuildSettings.development = true;
//                 BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
//                 EditorUtility.RevealInFinder(outputFile);
//             }
//
//             if (GUILayout.Button("打包 Steam 正式版"))
//             {
//                 mConfig.Debug = false;
//                 mConfig.PublishMode = GameConfig.PublishModes.Steam;
//                 Save();
//
//                 ResKitEditorAPI.BuildAssetBundles();
//                 PlayerSettings.bundleVersion = mConfig.Version;
//                 PlayerSettings.productName = "HiEggplant2";
//                 if (PlatformCheck.IsOSX)
//                 {
//                     var scenes = EditorBuildSettings.scenes;
//                     var outputFile = Application.dataPath + "/../Builds/HiEggplant2OSX/HiEggplant2.app";
//                     var target = BuildTarget.StandaloneOSX;
//                     var options = BuildOptions.None;
//                     EditorUserBuildSettings.development = false;
//                     BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
//
//                     EditorUtility.RevealInFinder(outputFile);
//                 }
//             }
//         }
//
//         public void OnUpdate()
//         {
//         }
//
//
//         public void OnHide()
//         {
//         }
//
//         public void OnWindowGUIEnd()
//         {
//         }
//
//         public void OnDispose()
//         {
//         }
//     }
// }
// #endif
}