#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFramework
{
    public static class BuildKitHelper
    {
        public static void DrawVersionEditor()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version:");
            PlayerSettings.bundleVersion = EditorGUILayout.TextField(PlayerSettings.bundleVersion);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        public static void BuildWindowsAndZip(string appName,string zipFileBaseName = null)
        {
            if (zipFileBaseName == null)
            {
                zipFileBaseName = appName;
            }
            
            var scenes = EditorBuildSettings.scenes;
            var outputFile = Application.dataPath + $"/../Builds/{zipFileBaseName}Windows/{appName}.exe";
            const BuildTarget target = BuildTarget.StandaloneWindows64;
            const BuildOptions options = BuildOptions.None;
            outputFile.GetFolderPath().CreateDirIfNotExists();
            EditorUserBuildSettings.development = false;
            BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
            var zipFilePath = Application.dataPath + $"/../Builds/{zipFileBaseName}Windows_{PlayerSettings.bundleVersion}.zip";
            ZipUtility.ZipFolder(outputFile.GetFolderPath(), zipFilePath);
            EditorUtility.RevealInFinder(zipFilePath);
        }

        public static void BuildWebGL()
        {
            var scenes = EditorBuildSettings.scenes;
            var outputFolder = Application.dataPath + "/../Builds/WebGL".CreateDirIfNotExists();
            outputFolder.DeleteDirIfExists();
            const BuildTarget target = BuildTarget.WebGL;
            const BuildOptions options = BuildOptions.None;
            EditorUserBuildSettings.development = false;
            BuildPipeline.BuildPlayer(scenes, outputFolder, target, options);
            EditorUtility.RevealInFinder(outputFolder);
        }
    }
}
#endif