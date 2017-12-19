/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
    using UnityEngine;

    using System.Collections.Generic;
    
    public class ResLoader : DisposableObject, IResLoader, IPoolAble, IPoolType
    {
        class CallBackWrap
        {
			private Action<bool, IRes>  mListener;
			private IRes                mRes;

            public CallBackWrap(IRes r, Action<bool, IRes> l)
            {
                mRes = r;
                mListener = l;
            }

            public void Release()
            {
                mRes.UnRegisteResListener(mListener);
            }

            public bool IsRes(IRes res)
            {
                if (res.AssetName == mRes.AssetName)
                {
                    return true;
                }
                return false;
            }
        }

		private List<IRes>                      mResArray = new List<IRes>();
		private LinkedList<IRes>                mWaitLoadList = new LinkedList<IRes>();
		private Action                          mListener;
		private IResLoaderStrategy              mStrategy;

		private bool                            mCacheFlag = false;
		private int                             mLoadingCount = 0;

		private LinkedList<CallBackWrap>        mCallbackRecardList;
		private static DefaultLoaderStrategy    sDefaultStrategy;

        public static IResLoaderStrategy defaultStrategy
        {
            get
            {
                if (sDefaultStrategy == null)
                {
                    sDefaultStrategy = new DefaultLoaderStrategy();
                }
                return sDefaultStrategy;
            }
        }

        public float Progress
        {
            get
            {
                if (mWaitLoadList.Count == 0)
                {
                    return 1;
                }

                float unit = 1.0f / mResArray.Count;
                float currentValue = unit * (mResArray.Count - mLoadingCount);

                LinkedListNode<IRes> currentNode = mWaitLoadList.First;

                while (currentNode != null)
                {
                    currentValue += unit * currentNode.Value.Progress;
                    currentNode = currentNode.Next;
                }

                return currentValue;
            }
        }

        public bool IsRecycled
        {
            get
            {
                return mCacheFlag;
            }

            set
            {
                mCacheFlag = value;
            }
        }

        public static ResLoader Allocate(IResLoaderStrategy strategy = null)
        {
			ResLoader loader = SafeObjectPool<ResLoader>.Instance.Allocate();
            loader.SetStrategy(strategy);
            return loader;
        }

        public ResLoader()
        {
            SetStrategy(sDefaultStrategy);
        }

        public void Add2Load(List<string> list)
        {
            if (list == null)
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; --i)
            {
                Add2Load(list[i]);
            }
        }

        public void Add2Load(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            if (string.IsNullOrEmpty(ownerBundle) || string.IsNullOrEmpty(assetName))
            {
                Log.E("Res Name Or Bundle Name Is Null.");
                return;
            }

            IRes res = FindResInArray(mResArray,ownerBundle,assetName);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerReward(res, listener);
                    res.RegisteResListener(listener);
                }
                return;
            }

            res = ResMgr.Instance.GetRes(ownerBundle,assetName, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerReward(res, listener);
                res.RegisteResListener(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            string[] depends = res.GetDependResList();

            if (depends != null)
            {
                for (int i = 0; i < depends.Length; ++i)
                {
                    Add2Load(depends[i]);
                }
            }

            AddRes2Array(res, lastOrder);
        }


        public void Add2Load(string assetName, Action<bool, IRes> listener = null, bool lastOrder = true)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.E("Res Name Is Null.");
                return;
            }

            IRes res = FindResInArray(mResArray, assetName);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerReward(res, listener);
                    res.RegisteResListener(listener);
                }
                return;
            }

            res = ResMgr.Instance.GetRes(assetName, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerReward(res, listener);
                res.RegisteResListener(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            string[] depends = res.GetDependResList();

            if (depends != null)
            {
                for (int i = 0; i < depends.Length; ++i)
                {
                    Add2Load(depends[i]);
                }
            }

            AddRes2Array(res, lastOrder);
        }

        public void LoadSync()
        {
            while (mWaitLoadList.Count > 0)
            {
                IRes first = mWaitLoadList.First.Value;
                --mLoadingCount;
                mWaitLoadList.RemoveFirst();

                if (first == null)
                {
                    return;
                }

                if (first.LoadSync())
                {
                    first.AcceptLoaderStrategySync(this, mStrategy);
                }
            }

            mStrategy.OnAllTaskFinish(this);
        }

        public T LoadAsset<T>(string ownerBundle, string assetName) where T : UnityEngine.Object
        {
            ownerBundle = ownerBundle.ToLower();
            if (!ownerBundle.EndsWith("_project_putaogame"))
            {
                ownerBundle += "_project_putaogame";
            }
            Add2Load(ownerBundle.ToLower(),assetName.ToLower());
            LoadSync();

            IRes res = ResMgr.Instance.GetRes(ownerBundle.ToLower(),assetName.ToLower(), false);
            if (res == null)
            {
                Log.E("Failed to Load Res:" + ownerBundle + assetName);
                return null;
            }
            return res.Asset as T;
        }

        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            return LoadSync(assetName) as T;
        }
        
        public UnityEngine.Object LoadSync(string name)
        {
            Add2Load(name);
            LoadSync();

            IRes res = ResMgr.Instance.GetRes(name, false);
            if (res == null)
            {
                Log.E("Failed to Load Res:" + name);
                return null;
            }

            return res.Asset;
        }

		#if UNITY_EDITOR
		Dictionary<string,Sprite> mCachedSpriteDict = new Dictionary<string,Sprite>();
		#endif
        
        public UnityEngine.Sprite LoadSprite(string bundleName,string spriteName)
        {
#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor) {
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    return mCachedSpriteDict[spriteName];
                }
                var texture = LoadAsset<Texture2D>(bundleName, spriteName);
                Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),UnityEngine.Vector2.one * 0.5f);
                mCachedSpriteDict.Add(spriteName,sprite);
                return mCachedSpriteDict[spriteName];
            }
            else 
#endif
            {
                return LoadAsset<Sprite>(bundleName,spriteName);
            }
        }
        

		public UnityEngine.Sprite LoadSprite(string spriteName)
		{
			#if UNITY_EDITOR
			if (AbstractRes.SimulateAssetBundleInEditor) {
			    if (mCachedSpriteDict.ContainsKey(spriteName))
			    {
			        return mCachedSpriteDict[spriteName];
			    }
				var texture = LoadSync (spriteName) as Texture2D;
				Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),UnityEngine.Vector2.one * 0.5f);
				mCachedSpriteDict.Add(spriteName,sprite);
				return mCachedSpriteDict[spriteName];
			}
			else 
			#endif
			{
				return LoadAsset<Sprite>(spriteName);
			}
		}


        public void LoadAsync(Action listener = null)
        {
            mListener = listener;
            //ResMgr.Instance.timeDebugger.Begin("LoadAsync");
            DoLoadAsync();
        }

        public void ReleaseRes(string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

			#if UNITY_EDITOR
			if (AbstractRes.SimulateAssetBundleInEditor) {
				if (mCachedSpriteDict.ContainsKey(name))
				{
					var sprite = mCachedSpriteDict[name];
					GameObject.Destroy(sprite);
					mCachedSpriteDict.Remove(name);
				}
			}
			#endif

			IRes res = ResMgr.Instance.GetRes(name, false);
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

            if (mResArray.Remove(res))
            {
                res.UnRegisteResListener(OnResLoadFinish);
                res.Release();
				ResMgr.Instance.SetResMapDirty();
            }
        }

        public void ReleaseRes(string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return;
            }

            for (int i = names.Length - 1; i >=0; --i)
            {
                ReleaseRes(names[i]);
            }
        }

        public void ReleaseAllRes()
        {
			#if UNITY_EDITOR
			if (AbstractRes.SimulateAssetBundleInEditor) {

				foreach(var spritePair in mCachedSpriteDict)
				{
					GameObject.Destroy(spritePair.Value);

				}
				mCachedSpriteDict.Clear();
			}
			#endif

            mListener = null;
            mLoadingCount = 0;
            mWaitLoadList.Clear();

            if(mResArray.Count > 0)
            {
                //确保首先删除的是AB，这样能对Asset的卸载做优化
                mResArray.Reverse();

                for (int i = mResArray.Count - 1; i >= 0; --i)
                {
                    mResArray[i].UnRegisteResListener(OnResLoadFinish);
                    mResArray[i].Release();
                }

                mResArray.Clear();
				ResMgr.Instance.SetResMapDirty();
            }

            RemoveAllCallbacks(true);
        }

        public void UnloadImage(bool flag)
        {
            if (mResArray.Count > 0)
            {
                for (int i = mResArray.Count - 1; i >= 0; --i)
                {
                    if (mResArray[i].UnloadImage(flag))
                    {
                        if(mWaitLoadList.Remove(mResArray[i]))
                        {
                            --mLoadingCount;
                        }

                        RemoveCallback(mResArray[i], true);

                        mResArray[i].UnRegisteResListener(OnResLoadFinish);
                        mResArray[i].Release();
                        mResArray.RemoveAt(i);
                    }
                }
				ResMgr.Instance.SetResMapDirty();
            }
        }

        public override void Dispose()
        {
            ReleaseAllRes();
            base.Dispose();
        }

        public void Dump()
        {
            for (int i = 0; i < mResArray.Count; ++i)
            {
                Log.I(mResArray[i].AssetName);
            }
        }

        private void SetStrategy(IResLoaderStrategy strategy)
        {
            mStrategy = strategy;
            if (mStrategy == null)
            {
                mStrategy = defaultStrategy;
            }
        }

        private void DoLoadAsync()
        {
            if (mLoadingCount == 0)
            {
                //ResMgr.Instance.timeDebugger.End();
                //ResMgr.Instance.timeDebugger.Dump(-1);
                if (mListener != null)
                {
                    Action callback = mListener;
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
                IRes res = currentNode.Value;
                nextNode = currentNode.Next;
                if (res.IsDependResLoadFinish())
                {
                    mWaitLoadList.Remove(currentNode);

                    if (res.State != ResState.Ready)
                    {
                        res.RegisteResListener(OnResLoadFinish);
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
            if (mCallbackRecardList != null)
            {
                LinkedListNode<CallBackWrap> current = mCallbackRecardList.First;
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
                        mCallbackRecardList.Remove(current);
                    }
                    current = next;
                }
            }
        }

        private void RemoveAllCallbacks(bool release)
        {
            if (mCallbackRecardList != null)
            {
                int count = mCallbackRecardList.Count;
                while (count > 0)
                {
                    --count;
                    if (release)
                    {
                        mCallbackRecardList.Last.Value.Release();
                    }
                    mCallbackRecardList.RemoveLast();
                }
            }
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            --mLoadingCount;

            res.AcceptLoaderStrategyAsync(this, mStrategy);
            DoLoadAsync();
            if (mLoadingCount == 0)
            {
                RemoveAllCallbacks(false);

                //ResMgr.Instance.timeDebugger.End();
                //ResMgr.Instance.timeDebugger.Dump(-1);
                if (mListener != null)
                {
                    mListener();
                    mListener = null;
                }

                mStrategy.OnAllTaskFinish(this);
            }
        }

        private void AddRes2Array(IRes res, bool lastOrder)
        {
            //再次确保队列中没有它
            IRes oldRes = FindResInArray(mResArray, res.AssetName);

            if (oldRes != null)
            {
                return;
            }

            res.Retain();
            mResArray.Add(res);

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
        
        private static IRes FindResInArray(List<IRes> list,string ownerBundleName, string assetName)
        {
            if (list == null)
            {
                return null;
            }

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].AssetName.Equals(assetName) && list[i].OwnerBundleName.Equals(ownerBundleName))
                {
                    return list[i];
                }
            }

            return null;
        }
        

        private static IRes FindResInArray(List<IRes> list, string name)
        {
            if (list == null)
            {
                return null;
            }

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].AssetName.Equals(name))
                {
                    return list[i];
                }
            }

            return null;
        }

        private void AddResListenerReward(IRes res, Action<bool, IRes> listener)
        {
            if (mCallbackRecardList == null)
            {
                mCallbackRecardList = new LinkedList<CallBackWrap>();
            }

            mCallbackRecardList.AddLast(new CallBackWrap(res, listener));
        }

        public void OnRecycled()
        {
            ReleaseAllRes();
        }

        public void Recycle2Cache()
        {
			SafeObjectPool<ResLoader>.Instance.Recycle(this);
        }
    }
}
