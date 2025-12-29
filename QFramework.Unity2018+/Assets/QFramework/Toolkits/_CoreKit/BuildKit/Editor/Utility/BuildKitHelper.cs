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
        
        public static void BuildWindowsAndZip(string name)
        {
            var scenes = EditorBuildSettings.scenes;
            var outputFile = Application.dataPath + $"/../Builds/{name}Windows/{name}.exe";
            const BuildTarget target = BuildTarget.StandaloneWindows64;
            const BuildOptions options = BuildOptions.None;
            outputFile.GetFolderPath().CreateDirIfNotExists();
            EditorUserBuildSettings.development = false;
            BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
            var zipFilePath = Application.dataPath + $"/../Builds/{name}Windows_{PlayerSettings.bundleVersion}.zip";
            ZipUtility.ZipFolder(outputFile.GetFolderPath(), zipFilePath);
            EditorUtility.RevealInFinder(zipFilePath);
        }
    }
}
#endif