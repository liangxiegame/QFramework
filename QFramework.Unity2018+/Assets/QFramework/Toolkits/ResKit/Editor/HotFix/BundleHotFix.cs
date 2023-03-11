
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.IO;
//using NUnit.Framework;

namespace QFramework
{

    public  class BundleHotFix : EditorWindow
    {
        static string m_BunleTargetPath = (Application.streamingAssetsPath  + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER );
        static string m_HotPath = Application.dataPath + "/../Hot/" + EditorUserBuildSettings.activeBuildTarget.ToString();

        string md5Path = "";
        string hotCount = "1";
        OpenFileName m_OpenFileName = null;
        static Dictionary<string, ABMD5> m_PackedMd5 = new Dictionary<string, ABMD5>();

        // [MenuItem("热更/打包热更包", false, 3)]
        static void Init()
        {
            BundleHotFix window = (BundleHotFix)EditorWindow.GetWindow(typeof(BundleHotFix), false, "热更包界面", true);
            window.Show();

            Debug.Log(m_BunleTargetPath);
            Debug.Log(m_HotPath);
   
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            md5Path = EditorGUILayout.TextField("ABMD5路径： ", md5Path, GUILayout.Width(350), GUILayout.Height(20));
            if (GUILayout.Button("选择版本ABMD5文件", GUILayout.Width(150), GUILayout.Height(30)))
            {
                m_OpenFileName = new OpenFileName();
                m_OpenFileName.structSize = Marshal.SizeOf(m_OpenFileName);
                m_OpenFileName.filter = "ABMD5文件(*.bytes)\0*.bytes";
                m_OpenFileName.file = new string(new char[256]);
                m_OpenFileName.maxFile = m_OpenFileName.file.Length;
                m_OpenFileName.fileTitle = new string(new char[64]);
                m_OpenFileName.maxFileTitle = m_OpenFileName.fileTitle.Length;
                m_OpenFileName.initialDir = (Application.dataPath + "/../Version").Replace("/", "\\");//默认路径
                m_OpenFileName.title = "选择MD5窗口";
                m_OpenFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (LocalDialog.GetSaveFileName(m_OpenFileName))
                {
       
                    md5Path = m_OpenFileName.file;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            hotCount = EditorGUILayout.TextField("热更补丁版本：", hotCount, GUILayout.Width(350), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("开始打热更包", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (!string.IsNullOrEmpty(md5Path) && md5Path.EndsWith(".bytes"))
                {
                   BuildScript.BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
                    ReadMD5Com(md5Path, hotCount);
              

                }
                DeleteMainfest();
                //加密AB包
                   //EncryptAB();
            }
        }

        static void ReadMD5Com(string abmd5Path, string hotCount)
        {
            m_PackedMd5.Clear();

            using (var streamReader = new StreamReader(abmd5Path))
            {
                JsonUtility.FromJson<List<ABMD5>>(streamReader.ReadToEnd()).ForEach(_ =>
                {
                    m_PackedMd5.Add(_.ABName, _);
                });
            }

            List<string> changeList = new List<string>();
            DirectoryInfo directory = new DirectoryInfo(m_BunleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    string name = files[i].Name;
                    string md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                    Debug.Log("生成MD5" + files[i].FullName);
                    ABMD5 abmd5 = null;
                    if (!m_PackedMd5.ContainsKey(name))
                    {
                        changeList.Add(name);
                    }
                    else
                    {
                        if (m_PackedMd5.TryGetValue(name, out abmd5))
                        {
                            if (md5 != abmd5.MD5)
                            {
                                changeList.Add(name);
                            }
                        }
                    }
                }

            }

            CopyAbAndGeneratJson(changeList, hotCount);
        }

        static void CopyAbAndGeneratJson(List<string> changeList, string hotCount)
        {
            if (!Directory.Exists(m_HotPath))
            {
                Directory.CreateDirectory(m_HotPath);
            }
            DeleteAllFile(m_HotPath);
            foreach (string str in changeList)
            {
                if (!str.EndsWith(".manifest"))
                {
                    File.Copy(m_BunleTargetPath + "/" + str, m_HotPath + "/" + str);
                }
            }

            //生成服务器Patch
            DirectoryInfo directory = new DirectoryInfo(m_HotPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            Pathces pathces = new Pathces();
            pathces.Version = 1;
            pathces.Files = new List<Patch>();
            for (int i = 0; i < files.Length; i++)
            {
                Patch patch = new Patch();
                patch.Md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                patch.Name = files[i].Name;
                patch.Size = files[i].Length / 1024.0f;
                patch.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
                //服务器资源路径
                patch.Url = "http://annuzhiting2.oss-cn-hangzhou.aliyuncs.com/AssetBundle/" + PlayerSettings.bundleVersion + "/" + hotCount + "/" + files[i].Name;
                pathces.Files.Add(patch);
            }

            BinarySerializeOpt.Xmlserialize(m_HotPath + "/Patch.xml", pathces);
         
        }


        //   [MenuItem("Tools/加密AB包")]
        public static void EncryptAB(string EncryptKey)
        {
            DirectoryInfo directory = new DirectoryInfo(m_BunleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    AES.AESFileEncrypt(files[i].FullName, EncryptKey);
                }
            }
            Debug.Log("加密完成！");
        }

        //  [MenuItem("Tools/解密AB包")]
        public static void DecrptyAB(string DecrptyKey)
        {
            DirectoryInfo directory = new DirectoryInfo(m_BunleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    AES.AESFileDecrypt(files[i].FullName, DecrptyKey);
                }
            }
            Debug.Log("解密完成！");
        }

        /// <summary>
        /// 删除指定文件目录下的所有文件
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static void DeleteAllFile(string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo directory = new DirectoryInfo(fullPath);
                FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    File.Delete(files[i].FullName);
                }
            }
        }


        static void DeleteMainfest()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(m_BunleTargetPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".manifest"))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }

   
    }
}


