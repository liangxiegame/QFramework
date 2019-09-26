/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018.3 liangxie
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
    using UnityEngine;
    using System.Net;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading;
    
    public delegate void OnDownloadFinished(string fileName, int download, int totalFileLenght);
    public delegate void OnDownloadError(string errorMsg);
    public delegate void OnDownloadProgress(int download, int totalFileLenght);
    public delegate void OnDownloadBegin(int totalLength);

    //http下载管理器
    [MonoSingletonPath("[Singleton]/HttpDownloader")]
    public class HttpDownloaderMgr : MonoSingleton<HttpDownloaderMgr>
    {
        public static string TimeOutError = "Time-Out";
        enum DownloadEvent
        {
            Begin = 0,      //开始
            Error = 1,      // 出错
            Progress = 2,   // 进度
            Finish = 3,     // 下载完毕
            DownloadOrExit = 4 // 如果不是wifi状态，要下载的目标文件大于指定值，提示用户继续或者退出
        }

        private const int BUFFER_SIZE = 1024 * 200;
        private const int TIME_OUT = 10;

        // 用于和网络子线程交互
        private List<DownloadEvent> mEvent = new List<DownloadEvent>();
        string mErrorMsg = string.Empty;

        private string m_Uri, m_SaveFile;

        private event OnDownloadFinished mOnFinished;
        private event OnDownloadError mOnError;
        private event OnDownloadProgress mOnProgress;
        private event OnDownloadBegin mOnDownloadBegin;

        byte[] m_Buffer = new byte[BUFFER_SIZE];
        const string mRequestLock = "WebRequestLock";
        HttpWebRequest mRequest;
        // 将返回数据写入本地文件流
        FileStream mFileStream;
        // http请求响应
        HttpWebResponse mWriteResponse;
        // 要下载文件的长度
        int mFileLength = 0;
        // 当前已经下了多少字节
        int mCurrentDownloadByte = 0;
        int mStartPosition = 0;
        // 临时文件名
        string mTmpFile;

        // 是否正在下载
        bool mIsDownloading = false;
        // 当前是否正在使用wifi
        bool mUseWifi = true;
        // 当前正在下载的任务数
        int mTaskCount = 0;
        // 最后一次网络返回，用于超时
        long mLastResponseTime = 0;

        // 在等待是否下载和退出
        bool mIsWaitDownloadOrExit;

        // 添加下载任务，目前只支持一个任务同时进行
        public bool AddDownloadTask(string uri, string localPath, OnDownloadProgress onProgress, OnDownloadError onError, OnDownloadFinished onFinshed, OnDownloadBegin onBegin = null)
        {
            if (mIsDownloading)
            {
                Log.E("HttpDownloaderMgr is busy!");
                return false;
            }

            if (string.IsNullOrEmpty(uri) == true)
            {
                Log.E("uri is empty");
                return false;
            }

            if (string.IsNullOrEmpty(localPath) == true)
            {
                Log.E("LocalPath is empty");
                return false;
            }

            if (onError == null || onFinshed == null)
            {
                Log.E("onError & onFinshed should not be null!");
                return false;
            }

            mOnProgress = onProgress;
            mOnError = onError;
            mOnFinished = onFinshed;
            mOnDownloadBegin = onBegin;

            m_Uri = uri;
            m_SaveFile = localPath;

            mTaskCount++;

            Log.I("[HttpDownload]about to download new data:" + m_Uri);

            return true;
        }

        public void WorkForground()
        {
            if (mIsWaitDownloadOrExit)
            {
                ShowDownloadOrExitPanel();
            }
        }

        void Update()
        {
            ProcessEvent();
            if (mIsDownloading)
            {
                long diffTime = DateTime.Now.Ticks / 10000000 - mLastResponseTime;
                if (diffTime > TIME_OUT)
                {
                    HandleError(TimeOutError);
                    return;
                }
            }

            if (mIsDownloading || mTaskCount == 0)
            {
                return;
            }

            mTaskCount--;
            mIsDownloading = true;
            InitDownloadInfo();
            AsyncDownfile();
        }

        private void ProcessEvent()
        {
            DownloadEvent downloadEvent;
            string errorMsg;
            lock (mEvent)
            {
                if (mEvent.Count == 0)
                {
                    return;
                }

                downloadEvent = mEvent[0];
                mEvent.RemoveAt(0);
                errorMsg = string.Format("{0},{1}", mErrorMsg, m_Uri);
            }

            if (downloadEvent == DownloadEvent.Error)
            {
                Log.E(errorMsg);

                mIsDownloading = false;

                if (mRequest != null)
                {
                    mRequest.Abort();
                    mRequest = null;
                }

                if (mFileStream != null)
                {
                    mFileStream.Close();
                }

                if (mOnError != null)
                {
                    mOnError(errorMsg);
                }
            }
            else if (downloadEvent == DownloadEvent.Begin)
            {
                if (mOnDownloadBegin != null)
                {
                    mOnDownloadBegin(mFileLength);
                    mOnDownloadBegin = null;
                }
            }
            else if (downloadEvent == DownloadEvent.Progress)
            {
                if (mOnProgress != null)
                {
                    mOnProgress(mCurrentDownloadByte, mFileLength);
                }
            }
            else if (downloadEvent == DownloadEvent.Finish)
            {
                if (mOnFinished != null)
                {
                    mIsDownloading = false;
                    mOnFinished(m_SaveFile, mCurrentDownloadByte, mFileLength);
                }
            }
            else if (downloadEvent == DownloadEvent.DownloadOrExit)
            {
                mIsWaitDownloadOrExit = true;
            }
        }

        private void InitDownloadInfo()
        {
            mRequest = null;
            // 将返回数据写入本地文件流
            mFileStream = null;
            // 要下载文件的长度
            mFileLength = 0;
            // 当前已经下了多少字节
            mCurrentDownloadByte = 0;
            mStartPosition = 0;
            // 临时文件名
            mTmpFile = null;
            mLastResponseTime = 0;
            mErrorMsg = string.Empty;
            mEvent.Clear();
            mIsWaitDownloadOrExit = false;
        }

        private void UpdateTimeOut()
        {
            long tick = DateTime.Now.Ticks;
            mLastResponseTime = tick / 10000000;
        }

        // 同步下载（非断点续传）
        private void AsyncDownfile()
        {
            mUseWifi = !(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
            // 创建本地文件
            mTmpFile = m_SaveFile + ".temp";

            CheckLocalFile();

            if (mStartPosition < 0)
            {
                HandleError("CheckLocalFile fail");
                return;
            }

            UpdateTimeOut();

            Thread startRequest = new Thread(StartHttpWebRequest);
            startRequest.Start();
        }

        private void HandleError(string errorMsg)
        {
            lock (mEvent)
            {
                mEvent.Clear();
                mEvent.Add(DownloadEvent.Error);
                mErrorMsg = errorMsg;
            }
        }

        // 检查本地是否已有文件。有则断点续传。
        private void CheckLocalFile()
        {
            mStartPosition = -1;
            try
            {
                if (File.Exists(mTmpFile))
                {
                    mFileStream = File.OpenWrite(mTmpFile);
                    mStartPosition = (int)mFileStream.Length;
                    if (mStartPosition > 0)
                    {
                        mStartPosition -= 1;
                    }
                    Log.I("exist tmp file:" + mTmpFile + ", m_StartPosition:" + mStartPosition + ", IsAsync:" + mFileStream.IsAsync);
                    mFileStream.Seek(mStartPosition, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
                }
                else
                {
                    mFileStream = new FileStream(mTmpFile, FileMode.Create);
                    mStartPosition = 0;
                    Log.I("NOT exist tmp file:" + mTmpFile + ", IsAsync:" + mFileStream.IsAsync);
                }
            }
            catch (Exception exception)
            {
                if (mFileStream != null)
                {
                    mFileStream.Close();
                }
                Log.E("CheckLocalFile error:" + exception.Message);
            }
        }

        private void OnResponeCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // 请求已经失效
            if (null == mRequest || request != mRequest)
            {
                return;
            }

            Log.I("[HttpDownload]connect responed.");

            try
            {
                mWriteResponse = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                if ((int)mWriteResponse.StatusCode >= 300)
                {
                    Log.E("StatusCode=" + mWriteResponse.StatusCode + ", desc=" + mWriteResponse.StatusDescription);

                    HandleError(mWriteResponse.StatusDescription);

                    return;
                }
            }
            catch (Exception exception)
            {
                HandleError(exception.Message);
                return;
            }

            UpdateTimeOut();

            // 要下载文件长度
            mFileLength = (int)mWriteResponse.ContentLength + mStartPosition;
            mCurrentDownloadByte = mStartPosition;

            // 启动回调
            lock (mEvent)
            {
                mEvent.Add(DownloadEvent.Begin);
            }

            //当用户使用移动网络时
            int limit = 1024 * 1024;//ServerConfigMgr.S.nonWifiLimit; // >指定值，提示玩家是否下载
            if (!mUseWifi && mFileLength > limit)
            {
                lock (mEvent)
                {
                    mEvent.Add(DownloadEvent.DownloadOrExit);
                    mIsDownloading = false;
                }
            }
            else
            {
                ReadData(mWriteResponse);
            }
        }

        private void ReadData(HttpWebResponse writeResponse)
        {
            // 开始读数据
            try
            {
                // 开始读数据
                Stream responseStream = writeResponse.GetResponseStream();
                responseStream.BeginRead(m_Buffer, 0, BUFFER_SIZE, new AsyncCallback(OnReadCallback), responseStream);
            }
            catch (Exception exception)
            {
                HandleError(exception.Message);
            }
        }

        // 读取http返回数据流（回调）
        private void OnReadCallback(IAsyncResult asyncResult)
        {
            Stream responseStream = (Stream)asyncResult.AsyncState;
            try
            {
                int readCount = responseStream.EndRead(asyncResult);
                if (readCount > 0)
                {
                    mCurrentDownloadByte += readCount;

                    // write to file
                    if (mFileStream == null)
                    {
                        mFileStream = new FileStream(mTmpFile, FileMode.Create);
                    }
                    mFileStream.Write(m_Buffer, 0, readCount);

                    UpdateTimeOut();

                    // 进度回调
                    lock (mEvent)
                    {
                        for (int i = mEvent.Count - 1; i >= 0; i--)
                        {
                            if (mEvent[i] == DownloadEvent.Progress)
                            {
                                mEvent.RemoveAt(i);
                            }
                        }

                        mEvent.Add(DownloadEvent.Progress);
                    }

                    // 继续读取
                    responseStream.BeginRead(m_Buffer, 0, BUFFER_SIZE, new AsyncCallback(OnReadCallback), responseStream);
                }
                else // 已经读完
                {
                    responseStream.Close();
                    mFileStream.Close();

                    if (File.Exists(m_SaveFile))
                    {
                        File.Delete(m_SaveFile);
                    }
                    File.Move(mTmpFile, m_SaveFile);
                    Log.I("Finished!! fileLength:" + mFileLength + ",Download byte:" + mCurrentDownloadByte);

                    // 进度回调
                    lock (mEvent)
                    {
                        mEvent.Clear();
                        mEvent.Add(DownloadEvent.Finish);
                    }
                }
            }
            catch (Exception exception)
            {
                HandleError(exception.Message);
            }
        }

        // 退出游戏回调，只在游戏结束时调用一次
        protected override void OnDestroy()
        {
            lock (mRequestLock)
            {
                mIsDownloading = false;

                if (mRequest != null)
                {
                    mRequest.Abort();
                    mRequest = null;

                    if (mFileStream != null)
                    {
                        mFileStream.Close();
                        mFileStream = null;
                    }
                }
            }

            HandleError("App Quit");
        }

        void ShowDownloadOrExitPanel()
        {
            mIsWaitDownloadOrExit = false;

            //string lengthInMB = string.Format("{0:f1}M", (float)m_FileLength / (1024 * 1024));
            /*
            MsgMgr.S.ShowBox(MsgBoxStyle.S_YesNo | MsgBoxStyle.IsModal, null,
                             TDLanguageTable.GetFormat("MSG_NotWifi_Download", lengthInMB),
                             TDLanguageTable.Get("UI_Download"),
                             TDLanguageTable.Get("UI_Leave_Game"),
                             () =>
                             {
                                 m_IsDownloading = true;
                                 UpdateTimeOut();
                                 ReadData(m_WriteResponse);
                             },
                            () =>
                            {
                                Application.Quit();
                            }
            );
            */
        }

        void StartHttpWebRequest()
        {
            try
            {
                lock (mRequestLock)
                {
                    if (mIsDownloading)
                    {
                        mRequest = (HttpWebRequest)WebRequest.Create(m_Uri);

                        if (mStartPosition > 0)
                        {
                            mRequest.AddRange(mStartPosition);
                        }

                        mRequest.KeepAlive = false;
                        mRequest.BeginGetResponse(new AsyncCallback(OnResponeCallback), mRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
                return;
            }
        }
    }
}
