/****************************************************************************
 * Copyright (c) 2017 ~ 2021.8 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public class ResLoader : DisposableObject, IResLoader,IPoolType,IPoolable
    {
        [Obsolete("请使用 ResLoader.Allocate() 获取 ResLoader 对象", true)]
        public ResLoader()
        {
        }

        /// <summary>
        /// ID:RKRL001 申请ResLoader对象 ResLoader.Allocate（IResLoaderStrategy strategy = null)
        /// </summary>
        /// <param name="strategy">加载策略</param>
        /// <returns></returns>
        public static ResLoader Allocate()
        {
            return SafeObjectPool<ResLoader>.Instance.Allocate();
        }

        /// <summary>
        /// ID:RKRL002 释放ResLoader对象 ResLoader.Recycle2Cache
        /// </summary>
        public void Recycle2Cache()
        {
            if (mObject2Unload != null)
            {
                foreach (var o in mObject2Unload)
                {
                    if (o)
                    {
                        ResUnloadHelper.DestroyObject(o);
                    }
                }

                mObject2Unload.Clear();
                tempDepends.Clear();    
                mObject2Unload = null;
            }

            SafeObjectPool<ResLoader>.Instance.Recycle(this);
        }

        private List<Object> mObject2Unload;

        public void AddObjectForDestroyWhenRecycle2Cache(Object obj)
        {
            if (mObject2Unload == null)
            {
                mObject2Unload = new List<Object>();
            }

            mObject2Unload.Add(obj);
        }

        /// <summary>
        /// ID:RKRL003 同步加载AssetBundle里的资源 ResLoader.LoadSync<T>(string ownerBundle,string assetBundle)
        /// </summary>
        /// <param name="ownerBundle">assetBundle名字</param>
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSync<T>(string ownerBundle, string assetName) where T :Object
        {
           
            if (typeof(T)== typeof(Sprite))
            {
                if (AssetBundlePathHelper.SimulationMode)
                {
                    return LoadSprite(ownerBundle, assetName) as T;
                }
                else
                {
                    var resSearchKeys = ResSearchKeys.Allocate(assetName, ownerBundle, typeof(T));
                    var retAsset = LoadResSync(resSearchKeys);
                    resSearchKeys.Recycle2Cache();
                    return retAsset.Asset as T;
                }


              
            }
            else
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, ownerBundle, typeof(T));
                var retAsset = LoadResSync(resSearchKeys);
                resSearchKeys.Recycle2Cache();

                return retAsset.Asset as T;
            }
           

        
        }

        /// <summary>
        /// ID:RKRL003 只通过资源名字进行同步加载 ResLoader.LoadSync<T>(string assetName)
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSync<T>(string assetName) where T :Object
        {
            if (typeof(T) == typeof(Sprite))
            {
                if (AssetBundlePathHelper.SimulationMode)
                {
                    return LoadSprite(assetName) as T;
                }
                else
                {
                    var resSearchKeys = ResSearchKeys.Allocate(assetName, null, typeof(T));
                    var retAsset = LoadResSync(resSearchKeys);
                    resSearchKeys.Recycle2Cache();
                    return retAsset.Asset as T;
                }
            }
            else
            {
                var resSearchKeys = ResSearchKeys.Allocate(assetName, null, typeof(T));
                var retAsset = LoadResSync(resSearchKeys);
                resSearchKeys.Recycle2Cache();
                return retAsset.Asset as T;
            }          
        }


        public System.Object LoadSync<Scene>(string assetName, Action action = null)
        {
            var resSearchRule = ResSearchKeys.Allocate(assetName);

            if (action!=null)
            {
                action();
            }
          

            if (ResFactory.AssetBundleSceneResCreator.Match(resSearchRule))
            {
                //加载的为场景
                IRes res = ResFactory.AssetBundleSceneResCreator.Create(resSearchRule);
#if UNITY_EDITOR
                if (AssetBundlePathHelper.SimulationMode)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle((res as AssetBundleSceneRes).AssetBundleName)[0];
                    if (!string.IsNullOrEmpty(path))
                    {

                        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, new UnityEngine.SceneManagement.LoadSceneParameters());
                        resSearchRule.Recycle2Cache();
                        tempDepends.Clear();
                        return null;
                    }

                }
                else
                {
                    LoadResSync(resSearchRule);
                    SceneManager.LoadScene(assetName);
                    resSearchRule.Recycle2Cache();
                    tempDepends.Clear();
                    return null;
                }

#else
                   LoadResSync(resSearchRule);
                        SceneManager.LoadScene(assetName);
                      resSearchRule.Recycle2Cache();
                         tempDepends.Clear();
                        return null;
#endif
            }
            else
            {
                resSearchRule.Recycle2Cache();
                tempDepends.Clear();
                Debug.LogError("资源名称错误！请检查资源名称是否正确或是否被标记！AssetName:" + assetName);
            }



            resSearchRule.Recycle2Cache();

            return null;
        }


        public System.Object LoadSync<Scene>(string ownerBundle,string assetName, Action action = null)
        {
            var resSearchRule = ResSearchKeys.Allocate(assetName, ownerBundle);

            if (action != null)
            {
                action();
            }

            if (ResFactory.AssetBundleSceneResCreator.Match(resSearchRule))
            {
                //加载的为场景
                IRes res = ResFactory.AssetBundleSceneResCreator.Create(resSearchRule);
#if UNITY_EDITOR
                if (AssetBundlePathHelper.SimulationMode)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle((res as AssetBundleSceneRes).AssetBundleName)[0];
                    if (!string.IsNullOrEmpty(path))
                    {

                        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, new UnityEngine.SceneManagement.LoadSceneParameters());
                        resSearchRule.Recycle2Cache();
                        tempDepends.Clear();
                        return null;
                    }

                }
                else
                {
                    LoadResSync(resSearchRule);
                    SceneManager.LoadScene(assetName);
                    resSearchRule.Recycle2Cache();
                    tempDepends.Clear();
                    return null;
                }

#else
                   LoadResSync(resSearchRule);
                        SceneManager.LoadScene(assetName);
                      resSearchRule.Recycle2Cache();
                         tempDepends.Clear();
                        return null;
#endif
            }
            else
            {
                resSearchRule.Recycle2Cache();
                tempDepends.Clear();
                Debug.LogError("资源名称错误！请检查资源名称是否正确或是否被标记！AssetName:" + assetName);
            }



            resSearchRule.Recycle2Cache();

            return null;
        }

        /// <summary>
        /// ID:RKRL003 只通过资源名字进行同步加载,
        /// </summary>
        /// <param name="name">资源名字</param>
        /// <returns></returns>
        public Object LoadSync(string name)
        {
           

            var resSearchRule = ResSearchKeys.Allocate(name);
            IRes  retAsset = LoadResSync(resSearchRule); ;
           
            resSearchRule.Recycle2Cache();

            tempDepends.Clear();

            return retAsset.Asset;
        }

        public IRes LoadResSync(ResSearchKeys resSearchKeys)
        {
            Add2Load(resSearchKeys);
            LoadSync();

            var res = ResMgr.Instance.GetRes(resSearchKeys, false);
            if (res == null)
            {
                Debug.LogError("Failed to Load Res:" + resSearchKeys);
                return null;
            }
            else
            {
                //清理缓存的依赖资源名称
                tempDepends.Clear();
            }

            return res;
        }

        private void LoadSync()
        {
            while (mWaitLoadList.Count > 0)
            {
                var first = mWaitLoadList.First.Value;
                --mLoadingCount;
                mWaitLoadList.RemoveFirst();

                if (first == null)
                {
                    return;
                }

                if (first.LoadSync())
                {
                }
            }
        }


      
        class CallBackWrap
        {
            private readonly Action<bool, IRes> mListener;
            private readonly IRes mRes;

            public CallBackWrap(IRes r, Action<bool, IRes> l)
            {
                mRes = r;
                mListener = l;
            }

            public void Release()
            {
                mRes.UnRegisteOnResLoadDoneEvent(mListener);
            }

            public bool IsRes(IRes res)
            {
                return res.AssetName == mRes.AssetName;
            }
        }

        private readonly List<IRes> mResList = new List<IRes>();
        private readonly LinkedList<IRes> mWaitLoadList = new LinkedList<IRes>();
        private System.Action mListener;

        private int mLoadingCount;

        private LinkedList<CallBackWrap> mCallbackRecordList;


        public float Progress
        {
            get
            {
                if (mWaitLoadList.Count == 0)
                {
                    return 1;
                }

                var unit = 1.0f / mResList.Count;
                var currentValue = unit * (mResList.Count - mLoadingCount);

                var currentNode = mWaitLoadList.First;

                while (currentNode != null)
                {
                    currentValue += unit * currentNode.Value.Progress;
                    currentNode = currentNode.Next;
                }

                return currentValue;
            }
        }


        public void Add2Load(List<string> list)
        {
            if (list == null)
            {
                return;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(list[i]);

                Add2Load(resSearchRule);

                resSearchRule.Recycle2Cache();
            }
        }

        public void Add2Load(string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName);
            Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }

        public void Add2Load<T>(string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName, null, typeof(T));
            Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }


        public void Add2Load(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName, ownerBundle);

            Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }

        public void Add2Load<T>(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName, ownerBundle, typeof(T));
            Add2Load(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }

        //依赖资源名称缓存，防止重复添加内存
        List<string> tempDepends = new List<string>();

        private void Add2Load(ResSearchKeys resSearchKeys, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var res = FindResInArray(mResList, resSearchKeys);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerRecord(res, listener);
                    res.RegisteOnResLoadDoneEvent(listener);
                }

                return;
            }

            res = ResMgr.Instance.GetRes(resSearchKeys, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerRecord(res, listener);
                res.RegisteOnResLoadDoneEvent(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            var depends = res.GetDependResList();

            if (depends != null)
            {
             
                foreach (var depend in depends)
                {
           
                    if (!tempDepends.Contains(depend))
                    {
                        var searchRule = ResSearchKeys.Allocate(depend, null, typeof(AssetBundle));
                      
                       tempDepends.Add(depend);                     
                        Add2Load(searchRule);
                        searchRule.Recycle2Cache();
                    }                  
                }
            }


           

            AddRes2Array(res, lastOrder);
        }


        private readonly Dictionary<string, Sprite> mCachedSpriteDict = new Dictionary<string, Sprite>();


        public Sprite LoadSprite(string bundleName, string spriteName)
        {
            if (AssetBundlePathHelper.SimulationMode)
            {
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    return mCachedSpriteDict[spriteName];
                }

                var texture = LoadSync<Texture2D>(bundleName, spriteName);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                mCachedSpriteDict.Add(spriteName, sprite);
                return mCachedSpriteDict[spriteName];
            }


            return LoadSync<Sprite>(bundleName, spriteName);
        }


        public Sprite LoadSprite(string spriteName)
        {

            if (AssetBundlePathHelper.SimulationMode)
            {
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    return mCachedSpriteDict[spriteName];
                }

                var texture = LoadSync(spriteName) as Texture2D;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                mCachedSpriteDict.Add(spriteName, sprite);
                return mCachedSpriteDict[spriteName];
            }


            return LoadSync<Sprite>(spriteName);
        }


        public void LoadAsync(System.Action listener = null)
        {
            mListener = listener;
            DoLoadAsync();
            tempDepends.Clear();
        }


        public AsyncOperation LoadAsync(string sceneName,LoadSceneMode loadSceneMode=LoadSceneMode.Single,System.Action listener = null)
        {
            mListener = listener;
        
            var resSearchRule = ResSearchKeys.Allocate(sceneName);

            if (ResFactory.AssetBundleSceneResCreator.Match(resSearchRule))
            {
                //加载的为场景
                IRes res = ResFactory.AssetBundleSceneResCreator.Create(resSearchRule);
#if UNITY_EDITOR
                if (AssetBundlePathHelper.SimulationMode)
                {

                    string path = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle((res as AssetBundleSceneRes).AssetBundleName)[0];
                    if (!string.IsNullOrEmpty(path))
                    {
                        LoadSceneParameters sceneParameters=new LoadSceneParameters();
                        sceneParameters.loadSceneMode = loadSceneMode;
                        tempDepends.Clear();
                        return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, sceneParameters);
                    }
                 
                }
                else
                {
                    DoLoadAsync();
                    tempDepends.Clear();
                  return  SceneManager.LoadSceneAsync(sceneName,loadSceneMode);
                }

#else
                DoLoadAsync();
                    tempDepends.Clear();
                return    SceneManager.LoadSceneAsync(sceneName,loadSceneMode);
#endif
            }
            else
            {
                Debug.LogError("场景名称错误！请检查名称是否正确或资源是否被标记！AssetName:" + sceneName);
            }


            return null;
        }



        public void ReleaseRes(string resName)
        {
            if (string.IsNullOrEmpty(resName))
            {
                return;
            }

            if (AssetBundlePathHelper.SimulationMode)
            {
                if (mCachedSpriteDict.ContainsKey(resName))
                {
                    var sprite = mCachedSpriteDict[resName];
                    Object.Destroy(sprite);
                    mCachedSpriteDict.Remove(resName);
                }
            }

            var resSearchRule = ResSearchKeys.Allocate(resName);

            var res = ResMgr.Instance.GetRes(resSearchRule);
            resSearchRule.Recycle2Cache();

            if (res == null)
            {
                return;
            }

            if (mWaitLoadList.Remove(res))
            {
                --mLoadingCount;
                if (mLoadingCount == 0)
                {
                    mListener = null;
                }
            }

            if (mResList.Remove(res))
            {
                res.UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                res.Release();
                ResMgr.Instance.ClearOnUpdate();
            }
        }

        public void ReleaseRes(string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return;
            }

            for (var i = names.Length - 1; i >= 0; --i)
            {
                ReleaseRes(names[i]);
            }
        }

        public void ReleaseAllRes()
        {
            if (AssetBundlePathHelper.SimulationMode)
            {
                foreach (var spritePair in mCachedSpriteDict)
                {
                    Object.Destroy(spritePair.Value);
                }

                mCachedSpriteDict.Clear();
            }

            mListener = null;
            mLoadingCount = 0;
            mWaitLoadList.Clear();

            if (mResList.Count > 0)
            {
                //确保首先删除的是AB，这样能对Asset的卸载做优化
                mResList.Reverse();

                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    mResList[i].UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                    mResList[i].Release();
                }

                mResList.Clear();

                if (!ResMgr.IsApplicationQuit)
                {
                    ResMgr.Instance.ClearOnUpdate();
                }
            }

            RemoveAllCallbacks(true);
        }

        public void UnloadAllInstantiateRes(bool flag)
        {
            if (mResList.Count > 0)
            {
                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    if (mResList[i].UnloadImage(flag))
                    {
                        if (mWaitLoadList.Remove(mResList[i]))
                        {
                            --mLoadingCount;
                        }

                        RemoveCallback(mResList[i], true);

                        mResList[i].UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                        mResList[i].Release();
                        mResList.RemoveAt(i);
                    }
                }

                ResMgr.Instance.ClearOnUpdate();
            }
        }

        public override void Dispose()
        {
            ReleaseAllRes();
            base.Dispose();
        }

        public void Dump()
        {
            foreach (var res in mResList)
            {
                Debug.Log(res.AssetName);
            }
        }


        private void DoLoadAsync()
        {
            if (mLoadingCount == 0)
            {
                if (mListener != null)
                {
                    var callback = mListener;
                    mListener = null;
                    callback();
                }

                return;
            }

            var nextNode = mWaitLoadList.First;
            LinkedListNode<IRes> currentNode = null;
            while (nextNode != null)
            {
                currentNode = nextNode;
                var res = currentNode.Value;
                nextNode = currentNode.Next;
                if (res.IsDependResLoadFinish())
                {
                    mWaitLoadList.Remove(currentNode);

                    if (res.State != ResState.Ready)
                    {
                        res.RegisteOnResLoadDoneEvent(OnResLoadFinish);
                        res.LoadAsync();
                    }
                    else
                    {
                        --mLoadingCount;
                    }
                }
            }
        }

        private void RemoveCallback(IRes res, bool release)
        {
            if (mCallbackRecordList != null)
            {
                var current = mCallbackRecordList.First;
                LinkedListNode<CallBackWrap> next = null;
                while (current != null)
                {
                    next = current.Next;
                    if (current.Value.IsRes(res))
                    {
                        if (release)
                        {
                            current.Value.Release();
                        }

                        mCallbackRecordList.Remove(current);
                    }

                    current = next;
                }
            }
        }

        private void RemoveAllCallbacks(bool release)
        {
            if (mCallbackRecordList != null)
            {
                var count = mCallbackRecordList.Count;
                while (count > 0)
                {
                    --count;
                    if (release)
                    {
                        mCallbackRecordList.Last.Value.Release();
                    }

                    mCallbackRecordList.RemoveLast();
                }
            }
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            --mLoadingCount;

            DoLoadAsync();
            if (mLoadingCount == 0)
            {
                RemoveAllCallbacks(false);

                if (mListener != null)
                {
                    mListener();
                }
            }
        }

        private void AddRes2Array(IRes res, bool lastOrder)
        {
            var searchRule = ResSearchKeys.Allocate(res.AssetName, res.OwnerBundleName, res.AssetType);

            //再次确保队列中没有它
            var oldRes = FindResInArray(mResList, searchRule);

            searchRule.Recycle2Cache();

            if (oldRes != null)
            {
                return;
            }

            res.Retain();
            mResList.Add(res);

        

            if (res.State != ResState.Ready)
            {
                ++mLoadingCount;
                if (lastOrder)
                {
                    mWaitLoadList.AddLast(res);
                }
                else
                {
                    mWaitLoadList.AddFirst(res);
                }
            }
        }

        private static IRes FindResInArray(List<IRes> list, ResSearchKeys resSearchKeys)
        {
            if (list == null)
            {
                return null;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (resSearchKeys.Match(list[i]))
                {
                    return list[i];
                }
            }

            return null;
        }

        private void AddResListenerRecord(IRes res, Action<bool, IRes> listener)
        {
            if (mCallbackRecordList == null)
            {
                mCallbackRecordList = new LinkedList<CallBackWrap>();
            }

            mCallbackRecordList.AddLast(new CallBackWrap(res, listener));
        }

        bool IPoolable.IsRecycled { get; set; }

        void IPoolable.OnRecycled()
        {
            ReleaseAllRes();
        }
    }
}