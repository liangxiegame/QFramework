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
    using System.Collections;

    public class AbstractRes : SimpleRC, IRes, IPoolAble
    {
#if UNITY_EDITOR
        static int mSimulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

        // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (mSimulateAssetBundleInEditor == -1)
                {
                    mSimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;
                }
                return mSimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mSimulateAssetBundleInEditor)
                {
                    mSimulateAssetBundleInEditor = newValue;
                    UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }
#endif


        protected string mAssetName;
        protected string mOwnerBundleName;
        private short mResState = ResState.Waiting;
        private bool mCacheFlag = false;
        protected UnityEngine.Object mAsset;
        private event Action<bool, IRes> mResListener;

        public string AssetName
        {
            get { return mAssetName; }
            set { mAssetName = value; }
        }


        public short State
        {
            get { return mResState; }
            set
            {
                mResState = value;
                if (mResState == ResState.Ready)
                {
                    NotifyResEvent(true);
                }
            }
        }

        public string OwnerBundleName
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }

        public float Progress
        {
            get
            {
                if (mResState == ResState.Loading)
                {
                    return CalculateProgress();
                }

                if (mResState == ResState.Ready)
                {
                    return 1;
                }

                return 0;
            }
        }

        protected virtual float CalculateProgress()
        {
            return 0;
        }

        public UnityEngine.Object Asset
        {
            get { return mAsset; }
            set { mAsset = value; }
        }

        public virtual object RawAsset
        {
            get { return null; }
        }

        public bool IsRecycled
        {
            get { return mCacheFlag; }

            set { mCacheFlag = value; }
        }

        public virtual void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public virtual void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public void RegisteResListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (mResState == ResState.Ready)
            {
                listener(true, this);
                return;
            }

            mResListener += listener;
        }

        public void UnRegisteResListener(Action<bool, IRes> listener)
        {
            if (listener == null)
            {
                return;
            }

            if (mResListener == null)
            {
                return;
            }

            mResListener -= listener;
        }

        protected void OnResLoadFaild()
        {
            mResState = ResState.Waiting;
            NotifyResEvent(false);
        }

        private void NotifyResEvent(bool result)
        {
            if (mResListener != null)
            {
                mResListener(result, this);
                mResListener = null;
            }
        }

        protected AbstractRes(string assetName)
        {
            mAssetName = assetName;
        }

        public AbstractRes()
        {

        }

        protected bool CheckLoadAble()
        {
            if (mResState == ResState.Waiting)
            {
                return true;
            }

            return false;
        }

        protected void HoldDependRes()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i], false);
                if (res != null)
                {
                    res.Retain();
                }
            }
        }

        protected void UnHoldDependRes()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i], false);
                if (res != null)
                {
                    res.Release();
                }
            }
        }

        #region 子类实现

        public virtual bool LoadSync()
        {
            return false;
        }

        public virtual void LoadAsync()
        {
        }

        public virtual string[] GetDependResList()
        {
            return null;
        }

        public bool IsDependResLoadFinish()
        {
            string[] depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return true;
            }

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i], false);
                if (res == null || res.State != ResState.Ready)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool UnloadImage(bool flag)
        {
            return false;
        }

        public bool ReleaseRes()
        {
            if (mResState == ResState.Loading)
            {
                return false;
            }

            if (mResState != ResState.Ready)
            {
                return true;
            }

            //Log.I("Release Res:" + mName);

            OnReleaseRes();

            mResState = ResState.Waiting;
            mResListener = null;
            return true;
        }

        protected virtual void OnReleaseRes()
        {

        }

        protected override void OnZeroRef()
        {
            if (mResState == ResState.Loading)
            {
                return;
            }

            ReleaseRes();
        }

        public virtual void Recycle2Cache()
        {

        }

        public virtual void OnRecycled()
        {
            mAssetName = null;
            mResListener = null;
        }

        public virtual IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            finishCallback();
            yield break;
        }

        #endregion
    }
}