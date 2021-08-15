/****************************************************************************
 * Copyright (c) 2021.1 ~ 4 liangxie
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [ExecuteInEditMode]
    // ReSharper disable once RequiredBaseTypesIsNotInherited
    public class SettingFromUnityToDll : ISettingFromUnity
    {
#if UNITY_EDITOR
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.WSAPlayer:
                    return "WSAPlayer";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
#endif
                case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                    return "Linux";
#if !UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5
			case BuildTarget.StandaloneOSXUniversal:
#else
                case BuildTarget.StandaloneOSX:
#endif
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif


        public string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return AssetBundleSettings.GetPlatformForAssetBundles(Application.platform);
#endif
        }

        public string[] GetAssetPathsFromAssetBundleAndAssetName(string abRAssetName, string assetName)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abRAssetName, assetName);
#else
            return null;
#endif
        }

        public Object LoadAssetAtPath(string assetPath, Type assetType)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#else
            return null;
#endif
        }

        public T LoadAssetAtPath<T>(string assetPath) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
        }

        public string PasswordField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR
            return EditorGUILayout.PasswordField(value, style, options);
#else
            return GUILayout.PasswordField(value, '*', style, options);
#endif
        }

        public string TextField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextField(value, style, options);
#else
            return GUILayout.TextField(value, style, options);
#endif
        }

        public string TextArea(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextArea(value, style, options);
#else
            return GUILayout.TextArea(value, style, options);
#endif
        }

        public ResDatas BuildEditorDataTable()
        {
            Log.I("Start BuildAssetDataTable!");
            var resDatas = new ResDatas();
            AddABInfo2ResDatas(resDatas);
            return resDatas;
        }

        #region 构建 AssetDataTable

        public string AssetPath2Name(string assetPath)
        {
            var startIndex = assetPath.LastIndexOf("/") + 1;

            var endIndex = assetPath.LastIndexOf(".");
            if (endIndex > 0)
            {
                var length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }

        #endregion

        public void AddABInfo2ResDatas(IResDatas assetBundleConfigFile,string[] abNames = null)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var assetBundleNames = abNames ?? AssetDatabase.GetAllAssetBundleNames();
            foreach (var abName in assetBundleNames)
            {
                var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
                AssetDataGroup group;
                var abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, out @group);
                if (abIndex < 0)
                {
                    continue;
                }

                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                foreach (var cell in assets)
                {
                    var type = AssetDatabase.GetMainAssetTypeAtPath(cell);

                    var code = type.ToCode();

                    @group.AddAssetData(cell.EndsWith(".unity")
                        ? new AssetData(AssetPath2Name(cell), ResLoadType.ABScene, abIndex, abName, code)
                        : new AssetData(AssetPath2Name(cell), ResLoadType.ABAsset, abIndex, abName, code));
                }
            }
#endif
        }


        private static string mPersistentDataPath;
        private static string mStreamingAssetsPath;
        private static string mPersistentDataPath4Res;
        private static string mPersistentDataPath4Photo;

        // 外部目录  
        public string PersistentDataPath
        {
            get
            {
                if (null == mPersistentDataPath)
                {
                    mPersistentDataPath = Application.persistentDataPath + "/";
                }

                return mPersistentDataPath;
            }
        }

        // 内部目录
        public string StreamingAssetsPath
        {
            get
            {
                if (null == mStreamingAssetsPath)
                {
#if UNITY_IPHONE && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
					mStreamingAssetsPath =
 Application.streamingAssetsPath + "/";//GetParentDir(Application.dataPath, 2) + "/BuildRes/";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#else
                    mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
                }

                return mStreamingAssetsPath;
            }
        }

        // 外部资源目录
        public string PersistentDataPath4Res
        {
            get
            {
                if (null == mPersistentDataPath4Res)
                {
                    mPersistentDataPath4Res = PersistentDataPath + "Res/";

                    if (!Directory.Exists(mPersistentDataPath4Res))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Res);
#if UNITY_IPHONE && !UNITY_EDITOR
						UnityEngine.iOS.Device.SetNoBackupFlag(mPersistentDataPath4Res);
#endif
                    }
                }

                return mPersistentDataPath4Res;
            }
        }

        // 外部头像缓存目录
        public string PersistentDataPath4Photo
        {
            get
            {
                if (null == mPersistentDataPath4Photo)
                {
                    mPersistentDataPath4Photo = PersistentDataPath + "Photos\\";

                    if (!Directory.Exists(mPersistentDataPath4Photo))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Photo);
                    }
                }

                return mPersistentDataPath4Photo;
            }
        }

        // 资源路径，优先返回外存资源路径
        public string GetResPathInPersistentOrStream(string relativePath)
        {
            string resPersistentPath = string.Format("{0}{1}", PersistentDataPath4Res, relativePath);
            if (File.Exists(resPersistentPath))
            {
                return resPersistentPath;
            }
            else
            {
                return StreamingAssetsPath + relativePath;
            }
        }

// 上一级目录
        public string GetParentDir(string dir, int floor = 1)
        {
            string subDir = dir;

            for (int i = 0; i < floor; ++i)
            {
                int last = subDir.LastIndexOf('/');
                subDir = subDir.Substring(0, last);
            }

            return subDir;
        }

        public void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {
            if (outResult == null)
            {
                return;
            }

            var dir = new DirectoryInfo(dirName);

            if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System", StringComparison.Ordinal) > -1)
            {
                return;
            }

            var fileInfos = dir.GetFiles(fileName);
            outResult.AddRange(fileInfos.Select(fileInfo => fileInfo.FullName));

            var dirInfos = dir.GetDirectories();
            foreach (var dinfo in dirInfos)
            {
                GetFileInFolder(dinfo.FullName, fileName, outResult);
            }
        }

        public string PathPrefix
        {
            get
            {
#if UNITY_EDITOR || UNITY_IOS
                return "file://";
#else
                return string.Empty;
#endif
            }
        }
#if UNITY_EDITOR
        const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

        public bool SimulationMode
        {
            get { return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true); }
            set { UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value); }
        }
#else
         public bool SimulationMode
         {
             get { return false; }
             set {  }
         }
#endif
    }
}