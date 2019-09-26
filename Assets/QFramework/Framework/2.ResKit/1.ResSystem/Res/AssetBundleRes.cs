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

namespace QF.Res
{
    using UnityEngine;
    using System.Collections;

    public class AssetBundleRes : Res
    {
        private bool                     mUnloadFlag = true;
        private string[]                 mDependResList;
        private AssetBundleCreateRequest mAssetBundleCreateRequest;

        public static AssetBundleRes Allocate(string name)
        {
            var res = SafeObjectPool<AssetBundleRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.InitAssetBundleName();
            }

            return res;
        }
        
        private void InitAssetBundleName()
        {
            mDependResList = ResDatas.Instance.GetAllDependenciesByUrl(AssetName);
        }

        public AssetBundle AssetBundle
        {
            get { return (AssetBundle) mAsset; }
            private set { mAsset = value; }
        }

        public override void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public override void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            State = ResState.Loading;

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
            }
            else
#endif
            {
                var url = ResKitUtil.AssetBundleName2Url(mAssetName);
                var bundle = AssetBundle.LoadFromFile(url);

                mUnloadFlag = true;

                if (bundle == null)
                {
                    Log.E("Failed Load AssetBundle:" + mAssetName);
                    OnResLoadFaild();
                    return false;
                }

                AssetBundle = bundle;
            }

            State = ResState.Ready;

            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            State = ResState.Loading;

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //开启的时候已经结束了
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                yield return null;
            }
            else
#endif
            {
                var url = ResKitUtil.AssetBundleName2Url(mAssetName);
                var abcR = AssetBundle.LoadFromFileAsync(url);

                mAssetBundleCreateRequest = abcR;
                yield return abcR;
                mAssetBundleCreateRequest = null;

                if (!abcR.isDone)
                {
                    Log.E("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                    OnResLoadFaild();
                    finishCallback();
                    yield break;
                }

                AssetBundle = abcR.assetBundle;
            }

            State = ResState.Ready;
            finishCallback();
        }

        public override string[] GetDependResList()
        {
            return mDependResList;
        }

        public override bool UnloadImage(bool flag)
        {
            if (AssetBundle != null)
            {
                mUnloadFlag = flag;
            }

            return true;
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetBundleRes>.Instance.Recycle(this);
        }

        public override void OnRecycled()
        {
            base.OnRecycled();
            mUnloadFlag = true;
            mDependResList = null;
        }

        protected override float CalculateProgress()
        {
            if (mAssetBundleCreateRequest == null)
            {
                return 0;
            }

            return mAssetBundleCreateRequest.progress;
        }

        protected override void OnReleaseRes()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(mUnloadFlag);
                AssetBundle = null;
            }
        }

        public override string ToString()
        {
            return "Type:AssetBundle\t {0}".FillFormat(base.ToString());
        }
    }
}