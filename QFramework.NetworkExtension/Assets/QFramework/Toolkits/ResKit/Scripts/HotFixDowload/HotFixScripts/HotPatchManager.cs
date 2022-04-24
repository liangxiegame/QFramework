using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{

    public class HotPatchManager : ISingleton
    {
        public static HotPatchManager Instance => SingletonProperty<HotPatchManager>.Instance;
        private HotPatchManager() {}

        private MonoBehaviour m_Mono;

        private string m_UnPackPath = Application.persistentDataPath + "/Origin";
        /// <summary>
        /// 热更资源加载目录，请与reskit加载目录保持一致
        /// </summary>
        private string m_DownLoadPath = Application.persistentDataPath + "/DownLoad";
        private string m_CurVersion;
        public string CurVersion
        {
            get { return m_CurVersion; }
        }
        private string m_CurPackName;
        private string m_ServerXmlPath = Application.persistentDataPath + "/ServerInfo.xml";
        private string m_LocalXmlPath = Application.persistentDataPath + "/LocalInfo.xml";
        private ServerInfo m_ServerInfo;
        private ServerInfo m_LocalInfo;
        private VersionInfo m_GameVersion;
        //当前热更Patches
        private Pathces m_CurrentPatches;
        public Pathces CurrentPatches
        {
            get { return m_CurrentPatches; }
        }
        //所有热更的东西
        private Dictionary<string, Patch> m_HotFixDic = new Dictionary<string, Patch>();
        //所有需要下载的东西
        private List<Patch> m_DownLoadList = new List<Patch>();
        //所有需要下载的东西的Dic
        private Dictionary<string, Patch> m_DownLoadDic = new Dictionary<string, Patch>();
        //服务器上的资源名对应的MD5，用于下载后MD5校验
        private Dictionary<string, string> m_DownLoadMD5Dic = new Dictionary<string, string>();
        //计算需要解压的文件
        private List<string> m_UnPackedList = new List<string>();
        //原包记录的MD5码
        private Dictionary<string, ABMD5> m_PackedMd5 = new Dictionary<string, ABMD5>();
        //服务器列表获取错误回调
        public Action ServerInfoError;
        //文件下载出错回调
        public Action<string> ItemError;
        //下载完成回调
        public Action LoadOver;
        //储存已经下载的资源
        public List<Patch> m_AlreadyDownList = new List<Patch>();
        //是否开始下载
        public bool StartDownload = false;
        //尝试重新下载次数
        private int m_TryDownCount = 0;
        private const int DOWNLOADCOUNT = 4;
        //当前正在下载的资源
        private DownLoadAssetBundle m_CurDownload = null;

        // 需要下载的资源总个数
        public int LoadFileCount = 0;
        // 需要下载资源的总大小 KB
        public float LoadSumSize = 0;
        //是否开始解压
        public bool StartUnPack = false;
        //解压文件总大小
        public float UnPackSumSize = 0;
        //已解压大小
        public float AlreadyUnPackSize = 0;

        public void Init(MonoBehaviour mono)
        {
            m_Mono = mono;
            ReadMD5();
        }

        /// <summary>
        /// 读取本地资源MD5码
        /// </summary>
        void ReadMD5()
        {
            m_PackedMd5.Clear();
            TextAsset md5 = Resources.Load<TextAsset>("ABMD5");
            if (md5 == null)
            {
                Debug.LogError("未读取到本地MD5");
                return;
            }


            JsonUtility.FromJson<List<ABMD5>>(md5.text).ForEach(_ =>
            {
                m_PackedMd5.Add(_.ABName, _);
            });
        }

        /// <summary>
        /// 计算需要解压的文件
        /// </summary>
        /// <returns></returns>
        public bool ComputeUnPackFile()
        {
#if UNITY_ANDROID
        if (!Directory.Exists(m_UnPackPath))
        {
            Directory.CreateDirectory(m_UnPackPath);
        }
        m_UnPackedList.Clear();
        foreach (string fileName in m_PackedMd5.Keys)
        {
            string filePath = m_UnPackPath + "/" + fileName;
            if (File.Exists(filePath))
            {
                string md5 = MD5Manager.Instance.BuildFileMd5(filePath);
                if (m_PackedMd5[fileName].MD5 != md5)
                {
                    m_UnPackedList.Add(fileName);
                }
            }
            else
            {
                m_UnPackedList.Add(fileName);
            }
        }

        foreach (string fileName in m_UnPackedList)
        {
            if (m_PackedMd5.ContainsKey(fileName))
            {
                UnPackSumSize += m_PackedMd5[fileName].ABSize;
            }
        }

        return m_UnPackedList.Count > 0;
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public float GetUnpackProgress()
        {
            return AlreadyUnPackSize / UnPackSumSize;
        }

        /// <summary>
        /// 开始解压文件
        /// </summary>
        /// <param name="callBack"></param>
        public void StartUnackFile(Action callBack)
        {
            StartUnPack = true;
            m_Mono.StartCoroutine(UnPackToPersistentDataPath(callBack));
        }

        /// <summary>
        /// 将包里的原始资源解压到本地
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator UnPackToPersistentDataPath(Action callBack)
        {
            foreach (string fileName in m_UnPackedList)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileName);
                unityWebRequest.timeout = 30;
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.isNetworkError)
                {
                    Debug.Log("UnPack Error" + unityWebRequest.error);
                }
                else
                {
                    byte[] bytes = unityWebRequest.downloadHandler.data;
                    FileTool.CreateFile(m_UnPackPath + "/" + fileName, bytes);
                }

                if (m_PackedMd5.ContainsKey(fileName))
                {
                    AlreadyUnPackSize += m_PackedMd5[fileName].ABSize;
                }

                unityWebRequest.Dispose();
            }

            if (callBack != null)
            {
                callBack();
            }

            StartUnPack = false;
        }

        /// <summary>
        /// 计算AB包路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ComputeABPath(string name)
        {
            Patch patch = null;
            m_HotFixDic.TryGetValue(name, out patch);
            if (patch != null)
            {
                Debug.Log(m_DownLoadPath + "/" + name);

                return m_DownLoadPath + "/" + name;
            }
            return "";
        }

        /// <summary>
        /// 检测版本是否热更
        /// </summary>
        /// <param name="hotCallBack"></param>
        public void CheckVersion(Action<bool> hotCallBack = null)
        {
            m_TryDownCount = 0;
            m_HotFixDic.Clear();
            ReadVersion();
            m_Mono.StartCoroutine(ReadXml(() =>
            {
                if (m_ServerInfo == null)
                {
                    if (ServerInfoError != null)
                    {
                        ServerInfoError();
                    }
                    return;
                }

                foreach (VersionInfo version in m_ServerInfo.GameVersion)
                {
                    if (version.Version == m_CurVersion)
                    {
                        m_GameVersion = version;
                        break;
                    }
                }

                GetHotAB();
                if (CheckLocalAndServerPatch())
                {
                    ComputeDownload();
                    if (File.Exists(m_ServerXmlPath))
                    {
                        if (File.Exists(m_LocalXmlPath))
                        {
                            File.Delete(m_LocalXmlPath);
                        }
                        File.Move(m_ServerXmlPath, m_LocalXmlPath);
                    }
                }
                else
                {
                    ComputeDownload();
                }
                LoadFileCount = m_DownLoadList.Count;
                LoadSumSize = m_DownLoadList.Sum(x => x.Size);
                if (hotCallBack != null)
                {
                    hotCallBack(m_DownLoadList.Count > 0);
                }
            }));
        }

        /// <summary>
        /// 检查本地热更信息与服务器热更信息比较
        /// </summary>
        /// <returns></returns>
        bool CheckLocalAndServerPatch()
        {
            if (!File.Exists(m_LocalXmlPath))
                return true;

            m_LocalInfo = BinarySerializeOpt.XmlDeserialize(m_LocalXmlPath, typeof(ServerInfo)) as ServerInfo;

            VersionInfo localGameVesion = null;
            if (m_LocalInfo != null)
            {
                foreach (VersionInfo version in m_LocalInfo.GameVersion)
                {
                    if (version.Version == m_CurVersion)
                    {
                        localGameVesion = version;
                        break;
                    }
                }
            }

            if (localGameVesion != null && m_GameVersion.Pathces != null && localGameVesion.Pathces != null && m_GameVersion.Pathces.Length > 0 && m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1].Version != localGameVesion.Pathces[localGameVesion.Pathces.Length - 1].Version)
                return true;

            return false;
        }

        /// <summary>
        /// 读取打包时的版本
        /// </summary>
        void ReadVersion()
        {
            TextAsset versionTex = Resources.Load<TextAsset>("Version");
            if (versionTex == null)
            {
                Debug.LogError("未读到本地版本！");
                return;
            }
            string[] all = versionTex.text.Split('\n');
            if (all.Length > 0)
            {
                string[] infoList = all[0].Split(';');
                if (infoList.Length >= 2)
                {
                    m_CurVersion = infoList[0].Split('|')[1];
                    m_CurPackName = infoList[1].Split('|')[1];
                }
            }
        }

        IEnumerator ReadXml(Action callBack)
        {
            string xmlUrl = "http://annuzhiting2.oss-cn-hangzhou.aliyuncs.com/ServerInfo.xml";
       
            UnityWebRequest webRequest = UnityWebRequest.Get(xmlUrl);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Download Error" + webRequest.error);
            }
            else
            {
                FileTool.CreateFile(m_ServerXmlPath, webRequest.downloadHandler.data);
                if (File.Exists(m_ServerXmlPath))
                {
                    m_ServerInfo = BinarySerializeOpt.XmlDeserialize(m_ServerXmlPath, typeof(ServerInfo)) as ServerInfo;
                }
                else
                {
                    Debug.LogError("热更配置读取错误！");
                }
            }

            if (callBack != null)
            {
                callBack();
            }
        }

        /// <summary>
        /// 获取所有热更包信息
        /// </summary>
        void GetHotAB()
        {
            if (m_GameVersion != null && m_GameVersion.Pathces != null && m_GameVersion.Pathces.Length > 0)
            {
                Pathces lastPatches = m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1];
                if (lastPatches != null && lastPatches.Files != null)
                {
                    foreach (Patch patch in lastPatches.Files)
                    {
                        m_HotFixDic.Add(patch.Name, patch);
                    }
                }
            }
        }

        /// <summary>
        /// 计算下载的资源
        /// </summary>
        void ComputeDownload()
        {
            m_DownLoadList.Clear();
            m_DownLoadDic.Clear();
            m_DownLoadMD5Dic.Clear();
            if (m_GameVersion != null && m_GameVersion.Pathces != null && m_GameVersion.Pathces.Length > 0)
            {
                m_CurrentPatches = m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1];
                if (m_CurrentPatches.Files != null && m_CurrentPatches.Files.Count > 0)
                {
                    foreach (Patch patch in m_CurrentPatches.Files)
                    {
                        if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("StandaloneWindows64"))
                        {
                            AddDownLoadList(patch);
                        }
                        else if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("Android"))
                        {
                            AddDownLoadList(patch);
                        }
                        else if ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("IOS"))
                        {
                            AddDownLoadList(patch);
                        }
                    }
                }
            }
        }

        void AddDownLoadList(Patch patch)
        {
            string filePath = m_DownLoadPath + "/" + patch.Name;
            //存在这个文件时进行对比看是否与服务器MD5码一致，不一致放到下载队列，如果不存在直接放入下载队列
            if (File.Exists(filePath))
            {
                string md5 = MD5Manager.Instance.BuildFileMd5(filePath);
                Debug.Log(filePath);
                if (patch.Md5 != md5)
                {
                    m_DownLoadList.Add(patch);
                    m_DownLoadDic.Add(patch.Name, patch);
                    m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
                }
            }
            else
            {
                m_DownLoadList.Add(patch);
                m_DownLoadDic.Add(patch.Name, patch);
                m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
            }
        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            return GetLoadSize() / LoadSumSize;
        }

        /// <summary>
        /// 获取已经下载总大小
        /// </summary>
        /// <returns></returns>
        public float GetLoadSize()
        {
            float alreadySize = m_AlreadyDownList.Sum(x => x.Size);
            float curAlreadySize = 0;
            if (m_CurDownload != null)
            {
                Patch patch = FindPatchByGamePath(m_CurDownload.FileName);
                if (patch != null && !m_AlreadyDownList.Contains(patch))
                {
                    curAlreadySize = m_CurDownload.GetProcess() * patch.Size;
                }
            }

            return alreadySize + curAlreadySize;
        }

        /// <summary>
        /// 开始下载AB包
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public IEnumerator StartDownLoadAB(Action callBack, List<Patch> allPatch = null)
        {
            m_AlreadyDownList.Clear();
            StartDownload = true;
            if (allPatch == null)
            {
                allPatch = m_DownLoadList;
            }
            if (!Directory.Exists(m_DownLoadPath))
            {
                Directory.CreateDirectory(m_DownLoadPath);
            }

            List<DownLoadAssetBundle> downLoadAssetBundles = new List<DownLoadAssetBundle>();
            foreach (Patch patch in allPatch)
            {
                downLoadAssetBundles.Add(new DownLoadAssetBundle(patch.Url, m_DownLoadPath));
            }

            foreach (DownLoadAssetBundle downLoad in downLoadAssetBundles)
            {
                m_CurDownload = downLoad;
                yield return m_Mono.StartCoroutine(downLoad.Download());
                Patch patch = FindPatchByGamePath(downLoad.FileName);
                if (patch != null)
                {
                    m_AlreadyDownList.Add(patch);
                }
                downLoad.Destory();
            }

            //MD5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
            VerifyMD5(downLoadAssetBundles, callBack);
        }

        /// <summary>
        /// Md5码校验
        /// </summary>
        /// <param name="downLoadAssets"></param>
        /// <param name="callBack"></param>
        void VerifyMD5(List<DownLoadAssetBundle> downLoadAssets, Action callBack)
        {
            List<Patch> downLoadList = new List<Patch>();
            foreach (DownLoadAssetBundle downLoad in downLoadAssets)
            {
                string md5 = "";
                if (m_DownLoadMD5Dic.TryGetValue(downLoad.FileName, out md5))
                {
                    if (MD5Manager.Instance.BuildFileMd5(downLoad.SaveFilePath) != md5)
                    {
                       Debug.Log(downLoad.SaveFilePath);
                       //Debug.Log(m_DownLoadMD5Dic[downLoad.FileName] + "不同于"+ downLoad.SaveFilePath);

                   //     Debug.Log(string.Format("此文件{0}MD5校验失败，即将重新下载", downLoad.FileName));
                        Patch patch = FindPatchByGamePath(downLoad.FileName);
                        if (patch != null)
                        {
                            downLoadList.Add(patch);
                        }
                    }
                }
            }

            if (downLoadList.Count <= 0)
            {
                m_DownLoadMD5Dic.Clear();
                if (callBack != null)
                {
                    StartDownload = false;
                    callBack();
                }
                if (LoadOver != null)
                {
                    LoadOver();
                }
            }
            else
            {
                if (m_TryDownCount >= DOWNLOADCOUNT)
                {
                    string allName = "";
                    StartDownload = false;
                    foreach (Patch patch in downLoadList)
                    {
                        allName += patch.Name + ";";
                    }
                    Debug.LogError("资源重复下载4次MD5校验都失败，请检查资源" + allName);
                    if (ItemError != null)
                    {
                        ItemError(allName);
                    }
                }
                else
                {
                    m_TryDownCount++;
                    m_DownLoadMD5Dic.Clear();
                    foreach (Patch patch in downLoadList)
                    {
                        m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
                    }
                    //自动重新下载校验失败的文件
                    m_Mono.StartCoroutine(StartDownLoadAB(callBack, downLoadList));
                }
            }
        }

        /// <summary>
        /// 根据名字查找对象的热更Patch
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Patch FindPatchByGamePath(string name)
        {
            Patch patch = null;
            m_DownLoadDic.TryGetValue(name, out patch);
            return patch;
        }

        public void OnSingletonInit()
        {
            
        }
    }


    public class FileTool
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        public static void CreateFile(string filePath, byte[] bytes)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            FileInfo file = new FileInfo(filePath);
            Stream stream = file.Create();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
        }
    }

}