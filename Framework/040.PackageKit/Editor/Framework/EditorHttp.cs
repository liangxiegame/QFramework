/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{

    public class EditorHttpResponse
    {
        public ResponseType Type;

        public byte[] Bytes;

        public string Text;

        public string Error;
    }

    public enum ResponseType
    {
        SUCCEED,
        EXCEPTION,
        TIMEOUT,
    }

    public static class EditorHttp
    {
        public class EditorWWWExecuter
        {
            private WWW                        mWWW;
            private Action<EditorHttpResponse> mResponse;
            private Action<float>              mOnProgress;
            private bool                       mDownloadMode;

            public EditorWWWExecuter(WWW www, Action<EditorHttpResponse> response, Action<float> onProgress = null,
                bool downloadMode = false)
            {
                mWWW = www;
                mResponse = response;
                mOnProgress = onProgress;
                mDownloadMode = downloadMode;
                EditorApplication.update += Update;
            }

            void Update()
            {
                if (mWWW != null && mWWW.isDone)
                {
                    if (string.IsNullOrEmpty(mWWW.error))
                    {
                        if (mDownloadMode)
                        {
                            if (mOnProgress != null)
                            {
                                mOnProgress(1.0f);
                            }

                            mResponse(new EditorHttpResponse()
                            {
                                Type = ResponseType.SUCCEED,
                                Bytes = mWWW.bytes
                            });
                        }
                        else
                        {
                            mResponse(new EditorHttpResponse()
                            {
                                Type = ResponseType.SUCCEED,
                                Text = mWWW.text
                            });
                        }
                    }
                    else
                    {
                        mResponse(new EditorHttpResponse()
                        {
                            Type = ResponseType.EXCEPTION,
                            Error = mWWW.error
                        });
                    }

                    Dispose();
                }

                if (mWWW != null && mDownloadMode)
                {
                    if (mOnProgress != null)
                    {
                        mOnProgress(mWWW.progress);
                    }
                }
            }

            void Dispose()
            {
                mWWW.Dispose();
                mWWW = null;

                EditorApplication.update -= Update;
            }
        }


        public static void Get(string url, Action<EditorHttpResponse> response)
        {
            new EditorWWWExecuter(new WWW(url), response);
        }

        public static void Post(string url, WWWForm form, Action<EditorHttpResponse> response)
        {
            new EditorWWWExecuter(new WWW(url, form), response);
        }

        public static void Download(string url, Action<EditorHttpResponse> response, Action<float> onProgress = null)
        {
            new EditorWWWExecuter(new WWW(url), response, onProgress, true);
        }
    }
}