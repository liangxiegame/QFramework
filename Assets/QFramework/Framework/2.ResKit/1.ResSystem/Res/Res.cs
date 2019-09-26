/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

using QF.Extensions;
using UnityEngine;

namespace QF.Res
{
    using System;
    using System.Collections;

    public class Res : SimpleRC, IRes, IPoolable
    {
#if UNITY_EDITOR
        private static int    mSimulateAssetBundleInEditor = -1;
        const          string kSimulateAssetBundles        = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

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
        }
#endif


        protected string                 mAssetName;
        protected string                 mOwnerBundleName;
        private   ResState               mResState = ResState.Waiting;
        protected UnityEngine.Object     mAsset;
        private event Action<bool, IRes> mResListener;

        public string AssetName
        {
            get { return mAssetName; }
            protected set { mAssetName = value; }
        }


        public ResState State
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
                switch (mResState)
                {
                    case ResState.Loading:
                        return CalculateProgress();
                    case ResState.Ready:
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
        }

        public bool IsRecycled { get; set; }

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
            mResListener.InvokeGracefully(result, this);
            mResListener = null;
        }

        protected Res(string assetName)
        {
            IsRecycled = false;
            mAssetName = assetName;
        }

        public Res()
        {
            IsRecycled = false;
        }

        protected bool CheckLoadAble()
        {
            return mResState == ResState.Waiting;
        }

        protected void HoldDependRes()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchRule.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                
                if (res != null)
                {
                    res.Retain();
                }
            }
        }

        protected void UnHoldDependRes()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchRule.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                
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
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0)
            {
                return true;
            }

            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchRule.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                
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
            //如果Image 直接释放了，这里会直接变成NULL
            if (mAsset != null)
            {
                if (mAsset is GameObject)
                {

                }
                else
                {
                    Resources.UnloadAsset(mAsset);
                }

                mAsset = null;
            }
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

        public virtual IEnumerator DoLoadAsync(Action finishCallback)
        {
            finishCallback();
            yield break;
        }

        public override string ToString()
        {
            return "Name:{0}\t State:{1}\t RefCount:{2}".FillFormat(AssetName,State,RefCount);
        }

        #endregion
    }
}