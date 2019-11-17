/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.7 liangxie
 *
 * TODO:之后可能不用这个了
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


using QF;

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;
    using System.IO;
    using UniRx;

    public delegate void OnZipFinished(string zipFilePath, string outDirPath);

    public delegate void OnZipError(string zipFilePath, string outDirPath, string errorMsg);

    public delegate void OnZipProgress(string zipFilePath, string outDirPath, float percent);

    class ZipMgr : Singleton<ZipMgr>
    {
        class ZipWorker
        {
            string mZipFilePath;
            string mOutDirPath;
            event OnZipFinished mOnZipFinished;
            event OnZipError mOnZipError;
            event OnZipProgress mOnZipProgress;
            Thread mThread;
            bool mFinish;
            bool mIsError;
            string mErrorMsg;
            long mFileTotalCount;
            long mFileCompletedCount;
            long mCurFileProcessByteCount;
            long mCurFileTotalByteCount;

            public bool Finish
            {
                get { return mFinish; }
            }

            public ZipWorker(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error,
                OnZipProgress progress)
            {
                mZipFilePath = zipFilePath;
                mOutDirPath = outDirPath;
                mOnZipFinished = finished;
                mOnZipError = error;
                mOnZipProgress = progress;
                mThread = new Thread(Work);
                mFinish = false;
                mIsError = false;
                mErrorMsg = "";
                mFileTotalCount = 0;
                mFileCompletedCount = 0;
                mCurFileProcessByteCount = 0;
                mCurFileTotalByteCount = 0;
            }

            public void Update()
            {
                if (mIsError)
                {
                    if (mOnZipError != null)
                    {
                        mOnZipError(mZipFilePath, mOutDirPath, mErrorMsg);
                    }
                    mOnZipError = null;
                    mOnZipFinished = null;
                    mOnZipProgress = null;
                    return;
                }

                float percent = 0.0f;
                if (mFileTotalCount == 1)
                {
                    percent = (float) mCurFileProcessByteCount / (float) mCurFileTotalByteCount;
                    if (mCurFileProcessByteCount == 0)
                        percent = 0;
                    else if (mCurFileTotalByteCount == 0)
                        percent = 1f;
                }
                else
                {
                    percent = (float) mFileCompletedCount / (float) mFileTotalCount;
                    if (mFileCompletedCount == 0)
                        percent = 0;
                    else if (mFileTotalCount == 0)
                        percent = 1f;
                }

                //Debug.LogError(percent);
                if (mOnZipProgress != null)
                {
                    mOnZipProgress(mZipFilePath, mOutDirPath, percent);
                }

                if (mFinish)
                {
                    if (mOnZipFinished != null)
                    {
                        mOnZipFinished(mZipFilePath, mOutDirPath);
                    }
                    mOnZipError = null;
                    mOnZipFinished = null;
                    mOnZipProgress = null;
                }
            }

            public void Start()
            {
                mThread.Start();
            }

            public void Stop()
            {
                //m_Thread.Interrupt();
            }

            void Work()
            {
                try
                {
                    ZipFile zipFile = new ZipFile(mZipFilePath);
                    mFileTotalCount = zipFile.Count;
                    zipFile.Close();

                    FastZipEvents zipEvent = new FastZipEvents();
                    zipEvent.Progress = OnProcess;
                    zipEvent.CompletedFile = OnCompletedFile;

                    FastZip fastZip = new FastZip(zipEvent);
                    fastZip.CreateEmptyDirectories = true;
                    fastZip.ExtractZip(mZipFilePath, mOutDirPath, null);
                    mFinish = true;
                }
                catch (Exception exception)
                {
                    Log.E(exception.Message);
                    mErrorMsg = exception.Message;
                    mIsError = true;
                    mFinish = true;
                }
            }

            void OnProcess(object sender, ProgressEventArgs e)
            {
                mCurFileProcessByteCount = e.Processed;
                mCurFileTotalByteCount = e.Target > 0 ? e.Target : e.Processed;
            }

            void OnCompletedFile(object sender, ScanEventArgs e)
            {
                ++mFileCompletedCount;
            }
        }

        List<ZipWorker> mZipWorkerList = new List<ZipWorker>();

        public override void OnSingletonInit()
        {
            //启动 update 指令
            Observable.EveryUpdate().Subscribe(_ => Update());
        }

        private void Update()
        {
            for (int i = mZipWorkerList.Count - 1; i >= 0; --i)
            {
                mZipWorkerList[i].Update();
                if (mZipWorkerList[i].Finish)
                {
                    mZipWorkerList[i].Stop();
                    mZipWorkerList.RemoveAt(i);
                }
            }
        }

        public void UnZip(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error,
            OnZipProgress progress)
        {
            ZipWorker worker = new ZipWorker(zipFilePath, outDirPath, finished, error, progress);
            worker.Start();
            mZipWorkerList.Add(worker);
        }

        public bool UnZipData(byte[] inputData, string outDirPaht)
        {
            try
            {
                MemoryStream ms = new MemoryStream(inputData);
                FastZip fastZip = new FastZip();
                fastZip.CreateEmptyDirectories = true;
                fastZip.ExtractZip(ms, outDirPaht, FastZip.Overwrite.Always, null, null, null, false, true);
            }
            catch (System.Exception ex)
            {
                Log.E("Unzip Data error: " + ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        public byte[] UnZipData(byte[] inputData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(inputData))
                {
                    using (ZipFile zipFile = new ZipFile(ms))
                    {
                        ZipEntry zipEntry = zipFile[0];
                        byte[] outData = new byte[zipEntry.Size];
                        var stream = zipFile.GetInputStream(zipEntry);
                        stream.Read(outData, 0, outData.Length);
                        return outData;
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return null;
        }

        void Clean()
        {
            foreach (ZipWorker w in mZipWorkerList)
            {
                w.Stop();
            }
            mZipWorkerList.Clear();
        }
    }
}