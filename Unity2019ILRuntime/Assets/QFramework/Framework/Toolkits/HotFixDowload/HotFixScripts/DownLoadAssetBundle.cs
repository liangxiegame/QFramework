using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class DownLoadAssetBundle : DownLoadItem
    {
        UnityWebRequest m_WebRequest;

        public DownLoadAssetBundle(string url, string path) : base(url, path)
        {

        }

        public override IEnumerator Download(Action callback = null)
        {
            m_WebRequest = UnityWebRequest.Get(m_Url);
            m_StartDownLoad = true;
            m_WebRequest.timeout = 30;
            yield return m_WebRequest.SendWebRequest();
            m_StartDownLoad = false;

            if (m_WebRequest.isNetworkError)
            {
                Debug.LogError("Download Error" + m_WebRequest.error);
            }
            else
            {
                byte[] bytes = m_WebRequest.downloadHandler.data;
                FileTool.CreateFile(m_SaveFilePath, bytes);
                if (callback != null)
                {
                    callback();
                }
            }
        }

        public override void Destory()
        {
            if (m_WebRequest != null)
            {
                m_WebRequest.Dispose();
                m_WebRequest = null;
            }
        }

        public override long GetCurLength()
        {
            if (m_WebRequest != null)
            {
                return (long)m_WebRequest.downloadedBytes;
            }
            return 0;
        }

        public override long GetLength()
        {
            return 0;
        }

        public override float GetProcess()
        {
            if (m_WebRequest != null)
            {
                return (long)m_WebRequest.downloadProgress;
            }
            return 0;
        }
    }

}
