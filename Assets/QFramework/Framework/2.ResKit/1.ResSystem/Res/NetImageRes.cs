/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.7 liangxie
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

using QFramework;

namespace QF.Res
{
    using System;
    using UnityEngine;
    using System.Collections;
    
    public static class NetImageResUtil
    {
        public static string ToNetImageResName(this string selfHttpUrl)
        {
            return string.Format("NetImage:{0}", selfHttpUrl);
        }
    }
    
    public class NetImageRes : Res, IDownloadTask
    {
        private string mUrl;
        private string mHashCode;
        private object mRawAsset;
        private WWW mWWW;

        public static NetImageRes Allocate(string name)
        {
            NetImageRes res = SafeObjectPool<NetImageRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.SetUrl(name.Substring(9));
            }
            return res;
        }

        public void SetDownloadProgress(int totalSize, int download)
        {

        }

        public string LocalResPath
        {
            get { return string.Format("{0}{1}", FilePath.PersistentDataPath4Photo, mHashCode); }
        }

        public virtual object RawAsset
        {
            get { return mRawAsset; }
        }

        public bool NeedDownload
        {
            get { return RefCount > 0; }
        }

        public string Url
        {
            get { return mUrl; }
        }

        public int FileSize
        {
            get { return 1; }
        }

        public void SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            mUrl = url;
            mHashCode = string.Format("Photo_{0}", mUrl.GetHashCode());
        }

        public override bool UnloadImage(bool flag)
        {
            return false;
        }

        public override bool LoadSync()
        {
            return false;
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

            DoLoadWork();
        }

        private void DoLoadWork()
        {
            State = ResState.Loading;

            OnDownLoadResult(true);

            //检测本地文件是否存在
            /*
            if (!File.Exists(LocalResPath))
            {
                ResDownloader.S.AddDownloadTask(this);
            }
            else
            {
                OnDownLoadResult(true);
            }
            */
        }

        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                GameObject.Destroy(mAsset);
                mAsset = null;
            }

            mRawAsset = null;
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<NetImageRes>.Instance.Recycle(this);
        }

        public override void OnRecycled()
        {

        }

        public void DeleteOldResFile()
        {
            //throw new NotImplementedException();
        }

        public void OnDownLoadResult(bool result)
        {
            if (!result)
            {
                OnResLoadFaild();
                return;
            }

            if (RefCount <= 0)
            {
                State = ResState.Waiting;
                return;
            }

            ResMgr.Instance.PushIEnumeratorTask(this);
            //ResMgr.S.PostLoadTask(LoadImage());
        }

        //完全的WWW方式,Unity 帮助管理纹理缓存，并且效率貌似更高
        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            WWW www = new WWW(mUrl);

            mWWW = www;

            yield return www;

            mWWW = null;

            if (www.error != null)
            {
                Log.E(string.Format("Res:{0}, WWW Errors:{1}", mUrl, www.error));
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (!www.isDone)
            {
                Log.E("NetImageRes WWW Not Done! Url:" + mUrl);
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;

                yield break;
            }

            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;
                yield break;
            }

            //TimeDebugger dt = new TimeDebugger("Tex");
            //dt.Begin("LoadingTask");
            //这里是同步的操作
            mAsset = www.texture;
            //dt.End();

            www.Dispose();
            www = null;

            //dt.Dump(-1);

            State = ResState.Ready;

            finishCallback();
        }

        protected override float CalculateProgress()
        {
            if (mWWW == null)
            {
                return 0;
            }

            return mWWW.progress;
        }
    }
}
