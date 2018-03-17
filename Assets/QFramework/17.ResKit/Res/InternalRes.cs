/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
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
    using System.Collections;
    
    public class InternalRes : BaseRes
    {
        private ResourceRequest mResourceRequest;

        public static InternalRes Allocate(string name)
        {
            InternalRes res = SafeObjectPool<InternalRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
            }
            return res;
        }

        public static string Name2Path(string name)
        {
            return name.Substring(10);
        }

        public InternalRes(string assetName) : base(assetName)
        {

        }

        public InternalRes()
        {

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

            if (string.IsNullOrEmpty(mAssetName))
            {
                return false;
            }

            State = ResState.Loading;

            //TimeDebugger timer = ResMgr.Instance.timeDebugger;

            //timer.Begin("Resources.Load:" + mName);
            mAsset = Resources.Load(Name2Path(mAssetName));
            //timer.End();

            if (mAsset == null)
            {
                Log.E("Failed to Load Asset From Resources:" + Name2Path(mAssetName));
                OnResLoadFaild();
                return false;
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

            if (string.IsNullOrEmpty(mAssetName))
            {
                return;
            }

            State = ResState.Loading;

            ResMgr.Instance.PostIEnumeratorTask(this);
        }

        public override IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            ResourceRequest rQ = Resources.LoadAsync(Name2Path(mAssetName));

            mResourceRequest = rQ;
            yield return rQ;
            mResourceRequest = null;

            if (!rQ.isDone)
            {
                Log.E("Failed to Load Resources:" + mAssetName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            mAsset = rQ.asset;

            State = ResState.Ready;

            finishCallback();
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<InternalRes>.Instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (mResourceRequest == null)
            {
                return 0;
            }

            return mResourceRequest.progress;
        }
    }
}
