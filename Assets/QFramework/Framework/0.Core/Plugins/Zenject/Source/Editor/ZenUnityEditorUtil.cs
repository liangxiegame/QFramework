#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject.Internal
{
    public static class ZenUnityEditorUtil
    {
        // Returns true if succeeds without errors
        public static bool SaveThenRunPreserveSceneSetup(Action action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var originalSceneSetup = EditorSceneManager.GetSceneManagerSetup();

                try
                {
                    action();
                    return true;
                }
                catch (Exception e)
                {
                    Log.ErrorException(e);
                    return false;
                }
                finally
                {
                    EditorSceneManager.RestoreSceneManagerSetup(originalSceneSetup);
                }
            }

            return false;
        }

        // Feel free to call this method from an editor script, or a unit test, etc.
        // An exception will be thrown if any validation errors are encountered
        public static void ValidateCurrentSceneSetup()
        {
            bool encounteredError = false;

            Application.LogCallback logCallback = (condition, stackTrace, type) =>
            {
                if (type == LogType.Error || type == LogType.Assert
                    || type == LogType.Exception)
                {
                    encounteredError = true;
                }
            };

            Application.logMessageReceived += logCallback;

            try
            {
                Assert.That(!ProjectContext.HasInstance);
                ProjectContext.ValidateOnNextRun = true;

                foreach (var sceneContext in GetAllSceneContexts())
                {
                    sceneContext.Validate();
                }
            }
            catch (Exception e)
            {
                Log.ErrorException(e);
                encounteredError = true;
            }
            finally
            {
                Application.logMessageReceived -= logCallback;
            }

            if (encounteredError)
            {
                throw new ZenjectException("Zenject Validation Failed!  See errors below for details.");
            }
        }

        // NOTE: An exception will be thrown if any validation errors are encountered
        // Returns the number of scenes that successfully validated
        public static int ValidateAllActiveScenes()
        {
            var activeScenePaths = EditorBuildSettings.scenes.Where(x => x.enabled)
                .Select(x => x.path).ToList();

            foreach (var scenePath in activeScenePaths)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                ValidateCurrentSceneSetup();
            }

            return activeScenePaths.Count;
        }

        // Don't use this
        public static void RunCurrentSceneSetup()
        {
            Assert.That(!ProjectContext.HasInstance);

            foreach (var sceneContext in GetAllSceneContexts())
            {
                try
                {
                    sceneContext.Run();
                }
                catch (Exception e)
                {
                    // Add a bit more context
                    throw new ZenjectException(
                        "Scene '{0}' Failed To Start!".Fmt(sceneContext.gameObject.scene.name), e);
                }
            }
        }

        public static SceneContext GetSceneContextForScene(Scene scene)
        {
            var sceneContext = TryGetSceneContextForScene(scene);

            Assert.IsNotNull(sceneContext,
                "Could not find scene context for scene '{0}'", scene.name);

            return sceneContext;
        }

        public static SceneContext TryGetSceneContextForScene(Scene scene)
        {
            if (!scene.isLoaded)
            {
                return null;
            }

            var sceneContexts = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<SceneContext>()).ToList();

            if (sceneContexts.IsEmpty())
            {
                return null;
            }

            Assert.That(sceneContexts.Count == 1,
                "Found multiple SceneContexts in scene '{0}'.  Expected a maximum of one.", scene.name);

            return sceneContexts[0];
        }

        public static SceneDecoratorContext GetDecoratorContextForScene(Scene scene)
        {
            var decoratorContext = TryGetDecoratorContextForScene(scene);

            Assert.IsNotNull(decoratorContext,
                "Could not find decorator context for scene '{0}'", scene.name);

            return decoratorContext;
        }

        public static SceneDecoratorContext TryGetDecoratorContextForScene(Scene scene)
        {
            if (!scene.isLoaded)
            {
                return null;
            }

            var decoratorContexts = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<SceneDecoratorContext>()).ToList();

            if (decoratorContexts.IsEmpty())
            {
                return null;
            }

            Assert.That(decoratorContexts.Count == 1,
                "Found multiple DecoratorContexts in scene '{0}'.  Expected a maximum of one.", scene.name);

            return decoratorContexts[0];
        }

        static IEnumerable<SceneContext> GetAllSceneContexts()
        {
            var decoratedSceneNames = new List<string>();

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                var sceneContext = TryGetSceneContextForScene(scene);
                var decoratorContext = TryGetDecoratorContextForScene(scene);

                if (sceneContext != null)
                {
                    Assert.That(decoratorContext == null,
                        "Found both SceneDecoratorContext and SceneContext in the same scene '{0}'.  This is not allowed", scene.name);

                    decoratedSceneNames.RemoveAll(x => sceneContext.ContractNames.Contains(x));

                    yield return sceneContext;
                }
                else if (decoratorContext != null)
                {
                    Assert.That(!string.IsNullOrEmpty(decoratorContext.DecoratedContractName),
                        "Missing Decorated Contract Name on SceneDecoratorContext in scene '{0}'", scene.name);

                    decoratedSceneNames.Add(decoratorContext.DecoratedContractName);
                }
            }

            Assert.That(decoratedSceneNames.IsEmpty(),
                "Found decorator scenes without a corresponding scene to decorator.  Missing scene contracts: {0}", decoratedSceneNames.Join(", "));
        }

        public static string ConvertAssetPathToAbsolutePath(string assetPath)
        {
            return Path.Combine(
                Path.Combine(Path.GetFullPath(Application.dataPath), ".."), assetPath);
        }

        public static string ConvertFullAbsolutePathToAssetPath(string fullPath)
        {
            fullPath = Path.GetFullPath(fullPath);

            var assetFolderFullPath = Path.GetFullPath(Application.dataPath);

            if (fullPath.Length == assetFolderFullPath.Length)
            {
                Assert.IsEqual(fullPath, assetFolderFullPath);
                return "Assets";
            }

            var assetPath = fullPath.Remove(0, assetFolderFullPath.Length + 1).Replace("\\", "/");
            return "Assets/" + assetPath;
        }

        public static string GetCurrentDirectoryAssetPathFromSelection()
        {
            return ConvertFullAbsolutePathToAssetPath(
                GetCurrentDirectoryAbsolutePathFromSelection());
        }

        public static string GetCurrentDirectoryAbsolutePathFromSelection()
        {
            var folderPath = TryGetSelectedFolderPathInProjectsTab();

            if (folderPath != null)
            {
                return folderPath;
            }

            var filePath = TryGetSelectedFilePathInProjectsTab();

            if (filePath != null)
            {
                return Path.GetDirectoryName(filePath);
            }

            return Application.dataPath;
        }

        public static string TryGetSelectedFilePathInProjectsTab()
        {
            return GetSelectedFilePathsInProjectsTab().OnlyOrDefault();
        }

        public static List<string> GetSelectedFilePathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => File.Exists(x)).ToList();
        }

        public static List<string> GetSelectedAssetPathsInProjectsTab()
        {
            var paths = new List<string>();

            UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
                typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var item in selectedAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(item);

                if (!string.IsNullOrEmpty(assetPath))
                {
                    paths.Add(assetPath);
                }
            }

            return paths;
        }

        public static List<string> GetSelectedPathsInProjectsTab()
        {
            var paths = new List<string>();

            UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
                typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var item in selectedAssets)
            {
                var relativePath = AssetDatabase.GetAssetPath(item);

                if (!string.IsNullOrEmpty(relativePath))
                {
                    var fullPath = Path.GetFullPath(Path.Combine(
                        Application.dataPath, Path.Combine("..", relativePath)));

                    paths.Add(fullPath);
                }
            }

            return paths;
        }

        // Taken from http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
        public static void SaveScriptableObjectAsset(
            string path, ScriptableObject asset)
        {
            Assert.That(path.EndsWith(".asset"));

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        // Note that the path is relative to the Assets folder
        public static List<string> GetSelectedFolderPathsInProjectsTab()
        {
            return GetSelectedPathsInProjectsTab()
                .Where(x => Directory.Exists(x)).ToList();
        }

        // Returns the best guess directory in projects pane
        // Useful when adding to Assets -> Create context menu
        // Returns null if it can't find one
        // Note that the path is relative to the Assets folder for use in AssetDatabase.GenerateUniqueAssetPath etc.
        public static string TryGetSelectedFolderPathInProjectsTab()
        {
            return GetSelectedFolderPathsInProjectsTab().OnlyOrDefault();
        }
    }
}

#endif
