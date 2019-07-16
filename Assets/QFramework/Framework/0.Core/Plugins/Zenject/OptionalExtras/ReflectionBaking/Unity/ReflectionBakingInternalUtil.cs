using System;
using System.IO;
using System.Reflection;
using ModestTree;
using UnityEditor;
using UnityEngine;

namespace Zenject.ReflectionBaking
{
    public static class ReflectionBakingInternalUtil
    {
        public static string ConvertAssetPathToSystemPath(string assetPath)
        {
            string path = Application.dataPath;
            int pathLength = path.Length;
            path = path.Substring(0, pathLength - /* Assets */ 6);
            path = Path.Combine(path, assetPath);
            return path;
        }

        public static ZenjectReflectionBakingSettings TryGetEnabledSettingsInstance()
        {
            string[] guids = AssetDatabase.FindAssets("t:ZenjectReflectionBakingSettings");

            if (guids.IsEmpty())
            {
                return null;
            }

            ZenjectReflectionBakingSettings enabledSettings = null;

            foreach (var guid in guids)
            {
                var candidate = AssetDatabase.LoadAssetAtPath<ZenjectReflectionBakingSettings>(
                    AssetDatabase.GUIDToAssetPath(guid));

                if ((Application.isEditor && candidate.IsEnabledInEditor) || (BuildPipeline.isBuildingPlayer && candidate.IsEnabledInBuilds))
                {
                    Assert.IsNull(enabledSettings, "Found multiple enabled ZenjectReflectionBakingSettings objects!  Please disable/delete one to continue.");
                    enabledSettings = candidate;
                }
            }

            return enabledSettings;
        }

        public static string ConvertAbsoluteToAssetPath(string systemPath)
        {
            var projectPath = Application.dataPath;

            // Remove 'Assets'
            projectPath = projectPath.Substring(0, projectPath.Length - /* Assets */ 6);

            int systemPathLength = systemPath.Length;
            int assetPathLength = systemPathLength - projectPath.Length;

            Assert.That(assetPathLength > 0, "Unexpect path '{0}'", systemPath);

            return systemPath.Substring(projectPath.Length, assetPathLength);
        }

        public static void TryForceUnityFullCompile()
        {
            Type compInterface = typeof(UnityEditor.Editor).Assembly.GetType(
                "UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");

            if (compInterface != null)
            {
                var dirtyAllScriptsMethod = compInterface.GetMethod(
                    "DirtyAllScripts", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                dirtyAllScriptsMethod.Invoke(null, null);
            }

            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
