using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace QFramework
{
    [Serializable]
    public class MD5Manager  : ISingleton
    {
        public static MD5Manager Instance => SingletonProperty<MD5Manager>.Instance;
        private MD5Manager() {}

        //储存Md5码，filePath为文件路径，md5SavePath为储存md5码路径
        public void SaveMd5(string filePath, string md5SavePath)
        {
            string md5 = BuildFileMd5(filePath);
            string name = filePath + "_md5.dat";
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            StreamWriter sw = new StreamWriter(name, false, Encoding.UTF8);
            if (sw != null)
            {
                sw.Write(md5);
                sw.Flush();
                sw.Close();
            }
        }

        //储存Md5码，filePath为文件路径
        public void SaveMd5(string filePath)
        {
            string md5 = BuildFileMd5(filePath);
            string name = filePath + "_md5.dat";
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            StreamWriter sw = new StreamWriter(name, false, Encoding.UTF8);
            if (sw != null)
            {
                sw.Write(md5);
                sw.Flush();
                sw.Close();
            }
        }

        //获取之前储存的Md5码
        public string GetMd5(string path)
        {
            string name = path + "_md5.dat";
            try
            {
                StreamReader sr = new StreamReader(name, Encoding.UTF8);
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
            catch
            {
                return "";
            }
        }

        public string BuildFileMd5(string fliePath)
        {
            string filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(fliePath))
                {
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                                     
                    filemd5 = FormatMD5(fileMD5Bytes);

                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
            return filemd5;
        }

        public string FormatMD5(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();//将byte[]装换成字符串
        }

        public void OnSingletonInit()
        {
            
        }
    }
}

