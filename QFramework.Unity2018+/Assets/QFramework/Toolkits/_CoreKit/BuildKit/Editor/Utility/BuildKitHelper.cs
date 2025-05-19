#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class BuildKitHelper
    {
        public static void DrawVersionEditor()
        {
            GUILayout.Label("Version:");
            PlayerSettings.bundleVersion = EditorGUILayout.TextField(PlayerSettings.bundleVersion);
        }
        
        public static void BuildWindowsAndZIP(string name)
        {
            var scenes = EditorBuildSettings.scenes;
            var outputFile = Application.dataPath + $"/../Builds/{name}Windows/{name}.exe";
            var target = BuildTarget.StandaloneWindows64;
            var options = BuildOptions.None;
            outputFile.GetFolderPath().CreateDirIfNotExists();
            EditorUserBuildSettings.development = false;
            BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
            var zipFilePath = Application.dataPath + $"/../Builds/MyRoseWindows_{PlayerSettings.bundleVersion}.zip";
            ZipUtility.ZipFolder(outputFile.GetFolderPath(), zipFilePath);
            EditorUtility.RevealInFinder(zipFilePath);
        }
    }
}
#endif