/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("07.ResKit", "ResLoader API", 2, "ResLoader API")]
    [APIDescriptionCN("资源管理方案")]
    [APIDescriptionEN("Resource Managements Solution")]
#endif
    public static class IResLoaderExtensions
    {
        private static Type ComponentType = typeof(Component);
        private static Type GameObjectType = typeof(GameObject);
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("同步加载资源")]
        [APIDescriptionEN("Load Asset Sync")]
        [APIExampleCode(@"

var texture =mResLoader.LoadSync<Texture2D>(""MyAsset"");
// Or
texture = mResLoader.LoadSync<Texture2D>(""MyBundle"",""MyAsset"");
")]
#endif
        public static T LoadSync<T>(this IResLoader self, string assetName) where T : Object
        {
            var type = typeof(T);
            if (ComponentType.IsAssignableFrom(type))
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, null, GameObjectType);
                var retAsset = (self.LoadAssetSync(resSearchKeys) as GameObject)?.GetComponent<T>();
                resSearchKeys.Recycle2Cache();
                return retAsset;
            }
            else
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, null, type);
                var retAsset = self.LoadAssetSync(resSearchKeys) as T;
                resSearchKeys.Recycle2Cache();
                return retAsset;
            }
        }
        
        
        public static T LoadSync<T>(this IResLoader self, string ownerBundle, string assetName) where T : Object
        {
            var type = typeof(T);
            if (ComponentType.IsAssignableFrom(type))
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, ownerBundle, GameObjectType);
                var retAsset = (self.LoadAssetSync(resSearchKeys) as GameObject)?.GetComponent<T>();
                resSearchKeys.Recycle2Cache();
                return retAsset;
            }
            else
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, ownerBundle, type);
                var retAsset = self.LoadAssetSync(resSearchKeys) as T;
                resSearchKeys.Recycle2Cache();
                return retAsset;
            }
        }
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("异步加载资源")]
        [APIDescriptionEN("Load Asset Async")]
        [APIExampleCode(@"

mResLoader.Add2Load<Texture2D>(""MyAsset"");
// Or
mResLoader.Add2Load<Texture2D>(""MyBundle"",""MyAsset"");

mResLoader.LoadAsync(()=>
{
    // 此时不会触发加载，而是从缓存中获取资源
    // resources are fetched from the cache
    var texture = mResLoader.LoadSync<Texture2D>(""MyAsset"");
});
")]
#endif
        public static void Add2Load(this IResLoader self, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            
            var searchRule = ResSearchKeys.Allocate(assetName);
            self.Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }

        public static void Add2Load<T>(this IResLoader self, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var type = typeof(T);
            if (ComponentType.IsAssignableFrom(type))
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, null, GameObjectType);
                self.Add2Load(resSearchKeys, listener, lastOrder);
                resSearchKeys.Recycle2Cache();
            }
            else
            {
                var searchRule = ResSearchKeys.Allocate(assetName, null, type);
                self.Add2Load(searchRule, listener, lastOrder);
                searchRule.Recycle2Cache();
            }
        }


        public static void Add2Load(this IResLoader self, string ownerBundle, string assetName,
            Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName, ownerBundle);
            self.Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }

        public static void Add2Load<T>(this IResLoader self, string ownerBundle, string assetName,
            Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var type = typeof(T);
            if (ComponentType.IsAssignableFrom(type))
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, ownerBundle, GameObjectType);
                self.Add2Load(resSearchKeys, listener, lastOrder);
                resSearchKeys.Recycle2Cache();
            }
            else
            {
                var searchRule = ResSearchKeys.Allocate(assetName, ownerBundle, type);
                self.Add2Load(searchRule, listener, lastOrder);
                searchRule.Recycle2Cache();
            }
        }
        

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("同步加载场景")]
        [APIDescriptionEN("Load Scene Sync")]
        [APIExampleCode(@"
mResLoader.LoadSceneSync(""BattleScene"");
// Or 
mResLoader.LoadSceneSync(""BattleSceneBundle"",""BattleScene"");


mResLoader.LoadSceneSync(""BattleScene"",LoadSceneMode.Additive);
//
mResLoader.LoadSceneSync(""BattleScene"",LoadSceneMode.Additive,LocalPhysicsMode.Physics2D);
")]
#endif
        public static void LoadSceneSync(this IResLoader self, string assetName,
            LoadSceneMode mode = LoadSceneMode.Single,
            LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            var resSearchRule = ResSearchKeys.Allocate(assetName);
            self.LoadSceneSync(resSearchRule, mode, physicsMode);
            resSearchRule.Recycle2Cache();
        }

        public static void LoadSceneSync(this IResLoader self, string ownerBundle, string assetName,
            LoadSceneMode mode = LoadSceneMode.Single,
            LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            var resSearchRule = ResSearchKeys.Allocate(assetName, ownerBundle);
            self.LoadSceneSync(resSearchRule, mode, physicsMode);
            resSearchRule.Recycle2Cache();
        }

        public static void LoadSceneSync(this IResLoader self, ResSearchKeys resSearchRule,
            LoadSceneMode mode = LoadSceneMode.Single,
            LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            if (ResFactory.AssetBundleSceneResCreator.Match(resSearchRule))
            {
                //加载的为场景
                IRes res = ResFactory.AssetBundleSceneResCreator.Create(resSearchRule);
#if UNITY_EDITOR
                if (AssetBundlePathHelper.SimulationMode)
                {
                    string path =
                        UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle((res as AssetBundleSceneRes)
                            .AssetBundleName)[0];
                    if (!string.IsNullOrEmpty(path))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path,
                            new LoadSceneParameters(mode, physicsMode));
                    }
                }
                else
#endif
                {
                    self.LoadResSync(resSearchRule);
                    SceneManager.LoadScene(resSearchRule.OriginalAssetName, new LoadSceneParameters(mode, physicsMode));
                }
            }
            else
            {
                Debug.LogError("资源名称错误！请检查资源名称是否正确或是否被标记！AssetName:" + resSearchRule.AssetName);
            }
        }
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("异步加载场景")]
        [APIDescriptionEN("Load Scene Sync")]
        [APIExampleCode(@"
mResLoader.LoadSceneAsync(""BattleScene"");
// Or 
mResLoader.LoadSceneAsync(""BattleSceneBundle"",""BattleScene"");


mResLoader.LoadSceneAsync(""BattleScene"",LoadSceneMode.Additive);
//
mResLoader.LoadSceneAsync(""BattleScene"",LoadSceneMode.Additive,LocalPhysicsMode.Physics2D);


mResLoader.LoadSceneAsync(""BattleScene"",(operation)=>
{
    Debug.Log(operation.isDone);
});
")]
#endif
        public static void LoadSceneAsync(this IResLoader self, string sceneName,
            LoadSceneMode loadSceneMode =
                LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None,
            Action<AsyncOperation> onStartLoading = null)
        {

            var resSearchKey = ResSearchKeys.Allocate(sceneName);
            self.LoadSceneAsync(resSearchKey,loadSceneMode,physicsMode,onStartLoading);
            resSearchKey.Recycle2Cache();
        }
        
        public static void LoadSceneAsync(this IResLoader self, string bundleName,string sceneName,
            LoadSceneMode loadSceneMode =
                LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None,
            Action<AsyncOperation> onStartLoading = null)
        {

            var resSearchKey = ResSearchKeys.Allocate(bundleName,sceneName);
            self.LoadSceneAsync(resSearchKey,loadSceneMode,physicsMode,onStartLoading);
            resSearchKey.Recycle2Cache();
        }
        

        public static void LoadSceneAsync(this IResLoader self,ResSearchKeys resSearchKeys,
            LoadSceneMode loadSceneMode =
                LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None,
            Action<AsyncOperation> onStartLoading = null)
        {

            if (ResFactory.AssetBundleSceneResCreator.Match(resSearchKeys))
            {
                //加载的为场景
                var res = ResFactory.AssetBundleSceneResCreator.Create(resSearchKeys);
#if UNITY_EDITOR
                if (AssetBundlePathHelper.SimulationMode)
                {
                    var path =
                        UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle((res as AssetBundleSceneRes)
                            .AssetBundleName)[0];

                    if (!string.IsNullOrEmpty(path))
                    {
                        var sceneParameters = new LoadSceneParameters
                        {
                            loadSceneMode = loadSceneMode,
                            localPhysicsMode = physicsMode
                        };

                        var asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(
                            path,
                            sceneParameters);
                        onStartLoading?.Invoke(asyncOperation);
                    }
                }
                else
#endif
                {
                    var sceneName = resSearchKeys.OriginalAssetName;
                    
                    self.Add2Load(resSearchKeys,(b, res1) =>
                    {
                        var asyncOperation = SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters()
                        {
                            loadSceneMode = loadSceneMode,
                            localPhysicsMode = physicsMode
                        });
                        onStartLoading?.Invoke(asyncOperation);
                    });
                    self.LoadAsync();
                }
            }
            else
            {
                Debug.LogError("场景名称错误！请检查名称是否正确或资源是否被标记！AssetName:" + resSearchKeys.AssetName);
            }
        }

        [Obsolete("请使用 LoadSync<Sprite>,use LoadSync<Sprite> instead", true)]
        public static Sprite LoadSprite(this IResLoader self, string spriteName) => self.LoadSync<Sprite>(spriteName);

        [Obsolete("请使用 LoadSync<Sprite>,use LoadSync<Sprite> instead", true)]
        public static Sprite LoadSprite(this IResLoader self, string bundleName, string spriteName) =>
            self.LoadSync<Sprite>(bundleName, spriteName);
    }
}