using System;
using System.Collections.Generic;
using ModestTree;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject.Internal
{
    [InitializeOnLoad]
    public static class SceneParentAutomaticLoader
    {
        static SceneParentAutomaticLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                try
                {
                    ValidateMultiSceneSetupAndLoadDefaultSceneParents();
                }
                catch (Exception e)
                {
                    EditorApplication.isPlaying = false;
                    throw new ZenjectException(
                        "Failure occurred when attempting to load default scene parent contracts!", e);
                }
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                // It would be cool to restore the initial scene set up here but in order to do this
                // we would have to make sure that the user saves the scene before running which
                // would be too annoying, so just leave any changes we've made alone
            }
        }

        public static void ValidateMultiSceneSetupAndLoadDefaultSceneParents()
        {
            var defaultContractsMap = LoadDefaultContractsMap();

            // NOTE: Even if configs is empty we still want to do the below logic to validate the
            // multi scene setup

            var sceneInfos = GetLoadedZenjectSceneInfos();
            var contractMap = GetCurrentSceneContractsMap(sceneInfos);

            foreach (var sceneInfo in sceneInfos)
            {
                ProcessScene(sceneInfo, contractMap, defaultContractsMap);
            }
        }

        static Dictionary<string, LoadedSceneInfo> GetCurrentSceneContractsMap(
            List<LoadedSceneInfo> sceneInfos)
        {
            var contractMap = new Dictionary<string, LoadedSceneInfo>();

            foreach (var info in sceneInfos)
            {
                AddToContractMap(contractMap, info);
            }

            return contractMap;
        }

        static void ProcessScene(
            LoadedSceneInfo sceneInfo,
            Dictionary<string, LoadedSceneInfo> contractMap,
            Dictionary<string, string> defaultContractsMap)
        {
            if (sceneInfo.SceneContext != null)
            {
                Assert.IsNull(sceneInfo.DecoratorContext);
                ProcessSceneParents(sceneInfo, contractMap, defaultContractsMap);
            }
            else
            {
                Assert.IsNotNull(sceneInfo.DecoratorContext);
                ProcessSceneDecorators(sceneInfo, contractMap, defaultContractsMap);
            }
        }

        static void ProcessSceneDecorators(
            LoadedSceneInfo sceneInfo,
            Dictionary<string, LoadedSceneInfo> contractMap,
            Dictionary<string, string> defaultContractsMap)
        {
            var decoratedContractName = sceneInfo.DecoratorContext.DecoratedContractName;

            LoadedSceneInfo decoratedSceneInfo;

            if (contractMap.TryGetValue(decoratedContractName, out decoratedSceneInfo))
            {
                ValidateDecoratedSceneMatch(sceneInfo, decoratedSceneInfo);
                return;
            }

            decoratedSceneInfo = LoadDefaultSceneForContract(
                sceneInfo, decoratedContractName, defaultContractsMap);

            EditorSceneManager.MoveSceneAfter(decoratedSceneInfo.Scene, sceneInfo.Scene);

            ValidateDecoratedSceneMatch(sceneInfo, decoratedSceneInfo);

            ProcessScene(decoratedSceneInfo, contractMap, defaultContractsMap);
        }

        static void ProcessSceneParents(
            LoadedSceneInfo sceneInfo,
            Dictionary<string, LoadedSceneInfo> contractMap,
            Dictionary<string, string> defaultContractsMap)
        {
            foreach (var parentContractName in sceneInfo.SceneContext.ParentContractNames)
            {
                LoadedSceneInfo parentInfo;

                if (contractMap.TryGetValue(parentContractName, out parentInfo))
                {
                    ValidateParentChildMatch(parentInfo, sceneInfo);
                    continue;
                }

                parentInfo = LoadDefaultSceneForContract(sceneInfo, parentContractName, defaultContractsMap);

                AddToContractMap(contractMap, parentInfo);

                EditorSceneManager.MoveSceneBefore(parentInfo.Scene, sceneInfo.Scene);

                ValidateParentChildMatch(parentInfo, sceneInfo);

                ProcessScene(parentInfo, contractMap, defaultContractsMap);
            }
        }

        static LoadedSceneInfo LoadDefaultSceneForContract(
            LoadedSceneInfo sceneInfo, string contractName, Dictionary<string, string> defaultContractsMap)
        {
            string scenePath;

            if (!defaultContractsMap.TryGetValue(contractName, out scenePath))
            {
                throw Assert.CreateException(
                    "Could not fill contract '{0}' for scene '{1}'.  No scenes with that contract name are loaded, and could not find a match in any default scene contract configs to auto load one either."
                    .Fmt(contractName, sceneInfo.Scene.name));
            }

            Scene scene;

            try
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Error while attempting to load contracts for scene '{0}'".Fmt(sceneInfo.Scene.name), e);
            }

            return CreateLoadedSceneInfo(scene);
        }

        static void ValidateDecoratedSceneMatch(
            LoadedSceneInfo decoratorInfo, LoadedSceneInfo decoratedInfo)
        {
            var decoratorIndex = GetSceneIndex(decoratorInfo.Scene);
            var decoratedIndex = GetSceneIndex(decoratedInfo.Scene);
            var activeIndex = GetSceneIndex(EditorSceneManager.GetActiveScene());

            Assert.That(decoratorIndex < decoratedIndex,
                "Decorator scene '{0}' must be loaded before decorated scene '{1}'.  Please drag the decorator scene to be placed above the other scene in the scene hierarchy.",
                decoratorInfo.Scene.name, decoratedInfo.Scene.name);

            if (activeIndex > decoratorIndex)
            {
                EditorSceneManager.SetActiveScene(decoratorInfo.Scene);
            }
        }

        static void ValidateParentChildMatch(
            LoadedSceneInfo parentSceneInfo, LoadedSceneInfo sceneInfo)
        {
            var parentIndex = GetSceneIndex(parentSceneInfo.Scene);
            var childIndex = GetSceneIndex(sceneInfo.Scene);
            var activeIndex = GetSceneIndex(EditorSceneManager.GetActiveScene());

            Assert.That(parentIndex < childIndex,
                "Parent scene '{0}' must be loaded before child scene '{1}'.  Please drag it to be placed above its child in the scene hierarchy.", parentSceneInfo.Scene.name, sceneInfo.Scene.name);

            if (activeIndex > parentIndex)
            {
                EditorSceneManager.SetActiveScene(parentSceneInfo.Scene);
            }
        }

        static int GetSceneIndex(Scene scene)
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                if (EditorSceneManager.GetSceneAt(i) == scene)
                {
                    return i;
                }
            }

            throw Assert.CreateException();
        }

        static Dictionary<string, string> LoadDefaultContractsMap()
        {
            var configs = Resources.LoadAll<DefaultSceneContractConfig>(DefaultSceneContractConfig.ResourcePath);

            var map = new Dictionary<string, string>();

            foreach (var config in configs)
            {
                foreach (var info in config.DefaultContracts)
                {
                    if (info.ContractName.Trim().IsEmpty())
                    {
                        Log.Warn("Found empty contract name in default scene contract config at path '{0}'", AssetDatabase.GetAssetPath(config));
                        continue;
                    }

                    Assert.That(!map.ContainsKey(info.ContractName),
                        "Found duplicate contract '{0}' in default scene contract config at '{1}'!  Default contract already specified", info.ContractName, AssetDatabase.GetAssetPath(config));

                    map.Add(info.ContractName, AssetDatabase.GetAssetPath(info.Scene));
                }
            }

            return map;
        }

        static LoadedSceneInfo CreateLoadedSceneInfo(Scene scene)
        {
            var info = TryCreateLoadedSceneInfo(scene);
            Assert.IsNotNull(info, "Expected scene '{0}' to be a zenject scene", scene.name);
            return info;
        }

        static LoadedSceneInfo TryCreateLoadedSceneInfo(Scene scene)
        {
            var sceneContext = ZenUnityEditorUtil.TryGetSceneContextForScene(scene);
            var decoratorContext = ZenUnityEditorUtil.TryGetDecoratorContextForScene(scene);

            if (sceneContext == null && decoratorContext == null)
            {
                return null;
            }

            var info = new LoadedSceneInfo
            {
                Scene = scene
            };

            if (sceneContext != null)
            {
                Assert.IsNull(decoratorContext,
                "Found both SceneContext and SceneDecoratorContext in scene '{0}'", scene.name);

                info.SceneContext = sceneContext;
            }
            else
            {
                Assert.IsNotNull(decoratorContext);

                info.DecoratorContext = decoratorContext;
            }

            return info;
        }

        static List<LoadedSceneInfo> GetLoadedZenjectSceneInfos()
        {
            var result = new List<LoadedSceneInfo>();

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                var info = TryCreateLoadedSceneInfo(scene);

                if (info != null)
                {
                    result.Add(info);
                }
            }

            return result;
        }

        static void AddToContractMap(
            Dictionary<string, LoadedSceneInfo> contractMap, LoadedSceneInfo info)
        {
            if (info.SceneContext == null)
            {
                return;
            }

            foreach (var contractName in info.SceneContext.ContractNames)
            {
                LoadedSceneInfo currentInfo;

                if (contractMap.TryGetValue(contractName, out currentInfo))
                {
                    throw Assert.CreateException(
                        "Found multiple scene contracts with name '{0}'. Scene '{1}' and scene '{2}'",
                        contractName, currentInfo.Scene.name, info.Scene.name);
                }

                contractMap.Add(contractName, info);
            }
        }

        public class LoadedSceneInfo
        {
            public SceneContext SceneContext;
            public SceneDecoratorContext DecoratorContext;
            public Scene Scene;
        }
    }
}
