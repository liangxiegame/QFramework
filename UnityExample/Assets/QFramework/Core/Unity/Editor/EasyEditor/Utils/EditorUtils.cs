/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using Object = UnityEngine.Object;

    public class EditorUtils
    {
        //ѡ���ļ�����Ч:��Ҫѡ��ĳ���ļ�����
        public static string CurrentSelectPath
        {
            get
            {
                if (Selection.activeObject == null)
                {
                    return null;
                }
                return AssetDatabase.GetAssetPath(Selection.activeObject);
            }
        }

        public static string AssetsPath2ABSPath(string assetsPath)
        {
            string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
            return assetRootPath.Substring(0, assetRootPath.Length - 6) + assetsPath;
        }

        public static string ABSPath2AssetsPath(string absPath)
        {
            string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
			Debug.Log (assetRootPath);
			Debug.Log (System.IO.Path.GetFullPath (absPath));
            return "Assets" + System.IO.Path.GetFullPath(absPath).Substring(assetRootPath.Length).Replace("\\", "/");
        }


        public static string AssetPath2ReltivePath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Replace("Assets/", "");
        }

        public static bool ExcuteCmd(string toolName, string args, bool isThrowExcpetion = true)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = toolName;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            OuputProcessLog(process, isThrowExcpetion);
            return true;
        }

        public static void OuputProcessLog(System.Diagnostics.Process p, bool isThrowExcpetion)
        {
            string standardError = string.Empty;
            p.BeginErrorReadLine();

            p.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler((sender, outLine) =>
            {
                standardError += outLine.Data;
            });

            string standardOutput = string.Empty;
            p.BeginOutputReadLine();
            p.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler((sender, outLine) =>
            {
                standardOutput += outLine.Data;
            });

            p.WaitForExit();
            p.Close();

            Log.I(standardOutput);
            if (standardError.Length > 0)
            {
                if (isThrowExcpetion)
                {
                    Log.E(standardError);
                    throw new Exception(standardError);
                }
                else
                {
                    Log.I(standardError);
                }
            }
        }

        public static Dictionary<string, string> ParseArgs(string argString)
        {
            int curPos = argString.IndexOf('-');
            Dictionary<string, string> result = new Dictionary<string, string>();

            while (curPos != -1 && curPos < argString.Length)
            {
                int nextPos = argString.IndexOf('-', curPos + 1);
                string item = string.Empty;

                if (nextPos != -1)
                {
                    item = argString.Substring(curPos + 1, nextPos - curPos - 1);
                }
                else
                {
                    item = argString.Substring(curPos + 1, argString.Length - curPos - 1);
                }

                item = EditorUtils.StringTrim(item);
                int splitPos = item.IndexOf(' ');

                if (splitPos == -1)
                {
                    string key = EditorUtils.StringTrim(item);
                    result[key] = "";
                }
                else
                {
                    string key = EditorUtils.StringTrim(item.Substring(0, splitPos));
                    string value = EditorUtils.StringTrim(item.Substring(splitPos + 1, item.Length - splitPos - 1));
                    result[key] = value;
                }

                curPos = nextPos;
            }

            return result;
        }

        public static string GetFileMD5Value(string absPath)
        {
            if (!File.Exists(absPath))
                return "";

            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            FileStream file = new FileStream(absPath, System.IO.FileMode.Open);
            byte[] retVal = md5CSP.ComputeHash(file);
            file.Close();
            string result = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                result += retVal[i].ToString("x2");
            }

            return result;
        }

        public static string GetStrMD5Value(string str)
        {
            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            byte[] retVal = md5CSP.ComputeHash(Encoding.Default.GetBytes(str));
            string retStr = "";

            for (int i = 0; i < retVal.Length; i++)
            {
                retStr += retVal[i].ToString("x2");
            }

            return retStr;
        }

        public static List<Object> GetDirSubAssetsList(string dirAssetsPath, bool isRecursive = true, string suffix = "", bool isLoadAll = false)
        {
            string dirABSPath = ABSPath2AssetsPath(dirAssetsPath);
			Debug.Log (dirABSPath);
            List<string> assetsABSPathList = IOUtils.GetDirSubFilePathList(dirABSPath, isRecursive, suffix);
            List<Object> resultObjectList = new List<Object>();

            for (int i = 0; i < assetsABSPathList.Count; ++i)
            {
				Debug.Log (assetsABSPathList [i]);
                if (isLoadAll)
                {
                    Object[] objs = AssetDatabase.LoadAllAssetsAtPath(ABSPath2AssetsPath(assetsABSPathList[i]));
                    resultObjectList.AddRange(objs);
                }
                else
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(ABSPath2AssetsPath(assetsABSPathList[i]));
                    resultObjectList.Add(obj);
                }
            }

            return resultObjectList;
        }

        public static List<T> GetDirSubAssetsList<T>(string dirAssetsPath, bool isRecursive = true, string suffix = "", bool isLoadAll = false) where T : Object
        {
            List<T> result = new List<T>();
            List<Object> objectList = GetDirSubAssetsList(dirAssetsPath, isRecursive, suffix, isLoadAll);

            for (int i = 0; i < objectList.Count; ++i)
            {
                if (objectList[i] is T)
                {
                    result.Add(objectList[i] as T);
                }
            }

            return result;
        }

        public static string GetSelectedDirAssetsPath()
        {
            string path = string.Empty;

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }

        public static string StringTrim(string str, params char[] trimer)
        {
            int startIndex = 0;
            int endIndex = str.Length;

            for (int i = 0; i < str.Length; ++i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    startIndex = i;
                    break;
                }
            }

            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (!IsInCharArray(trimer, str[i]))
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex == 0 && endIndex == str.Length)
            {
                return string.Empty;
            }
            return str.Substring(startIndex, endIndex - startIndex + 1);
        }

        public static string StringTrim(string str)
        {
            return StringTrim(str, new char[] { ' ', '\t' });
        }

        static bool IsInCharArray(char[] array, char c)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == c)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ClearAssetBundlesName()
        {
            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];

            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }

            length = AssetDatabase.GetAllAssetBundleNames().Length;
            AssetDatabase.SaveAssets();
        }

        public static bool SetAssetBundleName(string assetsPath, string bundleName)
        {
            AssetImporter ai = AssetImporter.GetAtPath(assetsPath);

            if (ai != null)
            {
                ai.assetBundleName = bundleName + ".assetbundle";
                return true;
            }

            return false;
        }

        public static void SafeRemoveAsset(string assetsPath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetsPath);

            if (obj != null)
            {
                AssetDatabase.DeleteAsset(assetsPath);
            }
        }

        public static void Abort(string errMsg)
        {
            Log.E("BatchMode Abort Exit " + errMsg);
            System.Threading.Thread.CurrentThread.Abort();
            System.Diagnostics.Process.GetCurrentProcess().Kill();

            System.Environment.ExitCode = 1;
            System.Environment.Exit(1);

            EditorApplication.Exit(1);
        }
    }
}