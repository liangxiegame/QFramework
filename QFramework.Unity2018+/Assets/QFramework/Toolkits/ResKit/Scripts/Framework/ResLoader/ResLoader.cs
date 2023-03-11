/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNITY_EDITOR
    [ClassAPI("07.ResKit", "ResLoader Object", 1, "ResLoader Object")]
    [APIDescriptionCN("资源管理方案")]
    [APIDescriptionEN("Resource Managements Solution")]
#endif
    public class ResLoader : DisposableObject, IResLoader, IPoolType, IPoolable
    {
        [Obsolete("请使用 ResLoader.Allocate() 获取 ResLoader 对象", true)]
        public ResLoader()
        {
        }
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("获取 ResLoader")]
        [APIDescriptionEN("Get ResLoader")]
        [APIExampleCode(@"

public class MyScript : MonoBehaviour
{
    public ResLoader mResLoader = ResLoader.Allocate();

    ...
}
")]
#endif
        public static ResLoader Allocate()
        {
            return SafeObjectPool<ResLoader>.Instance.Allocate();
        }
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("归还 ResLoader")]
        [APIDescriptionEN("Recycle ResLoader")]
        [APIExampleCode(@"

public class MyScript : MonoBehaviour
{
    public ResLoader mResLoader = ResLoader.Allocate();

    ...
    void OnDestroy()
    {
        mResLoader.Recycle2Cache();
    }
}
")]
#endif
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
                mObject2Unload = null;
            }

            SafeObjectPool<ResLoader>.Instance.Recycle(this);
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
        
        private List<Object> mObject2Unload;

        public void AddObjectForDestroyWhenRecycle2Cache(Object obj)
        {
            if (mObject2Unload == null)
            {
                mObject2Unload = new List<Object>();
            }

            mObject2Unload.Add(obj);
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
        

        public void Add2Load(ResSearchKeys resSearchKeys, Action<bool, IRes> listener = null,
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
                    var searchRule = ResSearchKeys.Allocate(depend, null, typeof(AssetBundle));

                    Add2Load(searchRule);
                    searchRule.Recycle2Cache();
                }
            }

            AddRes2Array(res, lastOrder);
        }
        
        private readonly Dictionary<string, Sprite> mCachedSpriteDict = new Dictionary<string, Sprite>();

        public void LoadAsync(System.Action listener = null)
        {
            mListener = listener;
            DoLoadAsync();
        }
        
        public UnityEngine.Object LoadAssetSync(ResSearchKeys resSearchKeys)
        {
            UnityEngine.Object  retAsset = null;

            if (resSearchKeys.AssetType == typeof(Sprite))
            {
                if (AssetBundlePathHelper.SimulationMode)
                {
                    if (mCachedSpriteDict.ContainsKey(resSearchKeys.AssetName))
                    {
                        return mCachedSpriteDict[resSearchKeys.AssetName];
                    }

                    resSearchKeys.AssetType = typeof(Texture2D);
                    var texture = LoadResSync(resSearchKeys).Asset as  Texture2D;
                    resSearchKeys.AssetType = typeof(Sprite);
                        
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.one * 0.5f);
                    mCachedSpriteDict.Add(resSearchKeys.AssetName, sprite);
                    return mCachedSpriteDict[resSearchKeys.AssetName];
                }
                else
                {
                    retAsset = LoadResSync(resSearchKeys).Asset;
                }
            }
            else
            {
                retAsset = LoadResSync(resSearchKeys).Asset;
            }
            
            resSearchKeys.Recycle2Cache();

            return retAsset;
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

        public void ReleaseRes(params string[] names)
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