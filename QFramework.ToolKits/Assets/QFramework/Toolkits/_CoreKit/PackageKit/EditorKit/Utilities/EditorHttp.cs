/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
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
#pragma warning disable CS0618
            private WWW                        mWWW;
#pragma warning restore CS0618
            private Action<EditorHttpResponse> mResponse;
            private Action<float>              mOnProgress;
            private bool                       mDownloadMode;

#pragma warning disable CS0618
            public EditorWWWExecuter(WWW www, Action<EditorHttpResponse> response, Action<float> onProgress = null,
#pragma warning restore CS0618
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
#pragma warning disable CS0618
            new EditorWWWExecuter(new WWW(url), response);
#pragma warning restore CS0618
        }

        public static void Post(string url, WWWForm form, Action<EditorHttpResponse> response)
        {
#pragma warning disable CS0618
            new EditorWWWExecuter(new WWW(url, form), response);
#pragma warning restore CS0618
        }

        public static void Download(string url, Action<EditorHttpResponse> response, Action<float> onProgress = null)
        {
#pragma warning disable CS0618
            new EditorWWWExecuter(new WWW(url), response, onProgress, true);
#pragma warning restore CS0618
        }
    }
}
#endif