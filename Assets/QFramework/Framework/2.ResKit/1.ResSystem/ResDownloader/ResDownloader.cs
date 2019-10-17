/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
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

namespace QF.Res
{
    using System.Collections.Generic;

    public interface IDownloadTask
    {
        string LocalResPath
        {
            get;
        }

        bool NeedDownload
        {
            get;
        }

        string Url
        {
            get;
        }

        void DeleteOldResFile();

        void OnDownLoadResult(bool result);
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    public class ResDownloader : Singleton<ResDownloader>
    {
        private Dictionary<string, IDownloadTask> mAllDownloadTaskMap = new Dictionary<string, IDownloadTask>();
        private List<IDownloadTask> mWaitDownloadList = new List<IDownloadTask>();
        private IDownloadTask mDownloadingTask;//由于当前HttpDownloaderMgr只支持一个

        public bool HasDownloadTask(IDownloadTask res)
        {
            if (mAllDownloadTaskMap.ContainsKey(res.LocalResPath))
            {
                return true;
            }
            return false;
        }

        public void AddDownloadTask(IDownloadTask res)
        {
            if (res == null)
            {
                return;
            }

            //res.ResState = eResState.kWaitDownload;

            mAllDownloadTaskMap.Add(res.LocalResPath, res);
            mWaitDownloadList.Add(res);

            TryStartNextTask();
        }

        public bool RemoveDownloadTask(IDownloadTask res)
        {
            if (res == null)
            {
                return true;
            }

            //当前无法取消正在下载的资源
            /*
            if (res.ResState == eResState.kDownloading)
            {
                return false;
            }
            */

            if (!mAllDownloadTaskMap.ContainsKey(res.LocalResPath))
            {
                return true;
            }

            mAllDownloadTaskMap.Remove(res.LocalResPath);
            mWaitDownloadList.Remove(res);
            return true;
        }

        #region
        protected IDownloadTask PopNextTask()
        {
            for (int i = mWaitDownloadList.Count - 1; i >= 0; --i)
            {
                var res = mWaitDownloadList[i];

                if (res.NeedDownload)
                {
                    mWaitDownloadList.RemoveAt(i);
                    return res;
                }

                mWaitDownloadList[i].OnDownLoadResult(false);
                mAllDownloadTaskMap.Remove(res.LocalResPath);
                mWaitDownloadList.RemoveAt(i);

                //res.ResState = eResState.kNull;
            }
            return null;
        }

        protected void TryStartNextTask()
        {
            if (mDownloadingTask != null)
            {
                return;
            }

            if (mWaitDownloadList.Count == 0)
            {
                return;
            }

            IDownloadTask next = PopNextTask();
            if (next == null)
            {
                return;
            }

            //next.ResState = eResState.kDownloading;

            mDownloadingTask = next;

            HttpDownloaderMgr.Instance.AddDownloadTask(next.Url, next.LocalResPath, null, OnDownloadError, OnDownloadFinish, null);

            next.DeleteOldResFile();
        }

        private void RemoveTask(IDownloadTask res)
        {
            if (res == null)
            {
                return;
            }
            mAllDownloadTaskMap.Remove(res.LocalResPath);
        }

        private void OnDownloadError(string errorMsg)
        {
            if (mDownloadingTask == null)
            {
                TryStartNextTask();
                return;
            }

            Log.I("ResDownloader: Downloading Error:" + errorMsg);
            RemoveTask(mDownloadingTask);
            mDownloadingTask.OnDownLoadResult(false);
            mDownloadingTask = null;

            TryStartNextTask();
        }

        private void OnDownloadFinish(string fileName, int download, int totalFileLenght)
        {
            if (mDownloadingTask == null)
            {
                Log.E("ResDownloader: Error, Current Res Begin Download Is Null...");
                TryStartNextTask();
                return;
            }

            if (fileName != mDownloadingTask.LocalResPath)
            {
                Log.E("ResDownloader: Error, Not Current Res Begin Download...");
                mDownloadingTask = null;
                TryStartNextTask();
                return;
            }

            Log.I("ResDownloader: Downloading Success:" + fileName);
            RemoveTask(mDownloadingTask);

            mDownloadingTask.OnDownLoadResult(true);
            mDownloadingTask = null;
            TryStartNextTask();
        }

        #endregion
    }
}

