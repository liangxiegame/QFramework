/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2021.1 liangxie
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

// ReSharper disable once CheckNamespace
namespace QFramework
{
    using UnityEngine;
    using System.Collections;

    public enum InternalResNamePrefixType
    {
        Url, // resources://
        Folder, // Resources/
    }

    public class ResourcesRes : Res
    {
        private ResourceRequest mResourceRequest;

        private string mPath;

        public static ResourcesRes Allocate(string name, InternalResNamePrefixType prefixType)
        {
            var res = SafeObjectPool<ResourcesRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
            }

            if (prefixType == InternalResNamePrefixType.Url)
            {
                res.mPath = name.Substring("resources://".Length);
            }
            else
            {
                res.mPath = name.Substring("Resources/".Length);
            }

            return res;
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

            
            if (AssetType != null)
            {
                mAsset = Resources.Load(mPath,AssetType);
            }
            else
            {
                mAsset = Resources.Load(mPath);
            }
            
            
            if (mAsset == null)
            {
                Debug.LogError("Failed to Load Asset From Resources:" + mPath);
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

            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            ResourceRequest resourceRequest = null;

            if (AssetType != null)
            {
                resourceRequest = Resources.LoadAsync(mPath, AssetType);
            }
            else
            {
                resourceRequest = Resources.LoadAsync(mPath);
            }

            mResourceRequest = resourceRequest;
            yield return resourceRequest;
            mResourceRequest = null;

            if (!resourceRequest.isDone)
            {
                Debug.LogError("Failed to Load Resources:" + mAssetName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            mAsset = resourceRequest.asset;

            State = ResState.Ready;

            finishCallback();
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<ResourcesRes>.Instance.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (mResourceRequest == null)
            {
                return 0;
            }

            return mResourceRequest.progress;
        }

        public override string ToString()
        {
            return string.Format("Type:Resources {1}", AssetName, base.ToString());
        }
    }
}