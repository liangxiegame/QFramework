	
using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QFramework
{
    public class AssetBundleSettings
    {
        private static IResDatas mAssetBundleConfigFile = null;
        
        /// <summary>
        /// 默认
        /// </summary>
        private static Func<IResDatas> mAssetBundleConfigFileFactory = () =>  new ResDatas();

        public static Func<IResDatas> AssetBundleConfigFileFactory
        {
            set { mAssetBundleConfigFileFactory = value; }
        }

        /// <summary>
        /// 获取自定义的 资源信息
        /// </summary>
        /// <returns></returns>
        public static IResDatas AssetBundleConfigFile
        {
            get
            {
                if (mAssetBundleConfigFile == null)
                {
                    mAssetBundleConfigFile = mAssetBundleConfigFileFactory.Invoke();
                }

                return mAssetBundleConfigFile;
            }
            set { mAssetBundleConfigFile = value; }
        }


        public static bool LoadAssetResFromStreammingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadResFromStreammingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadResFromStreammingAssetsPath", value ? 1 : 0); }
        }
        
        
        
#if UNITY_EDITOR
        const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

        public static bool SimulateAssetBundleInEditor
        {
            get { return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true); }
            set { UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value); }
        }
#endif

        #region AssetBundle 相关

        public static string AssetBundleUrl2Name(string url)
        {
            string retName = null;
            string parren = FilePath.StreamingAssetsPath + "AssetBundles/" + GetPlatformName() + "/";
            retName = url.Replace(parren, "");

            parren = FilePath.PersistentDataPath + "AssetBundles/" + GetPlatformName() + "/";
            retName = retName.Replace(parren, "");
            return retName;
        }

        public static string AssetBundleName2Url(string name)
        {
            string retUrl = FilePath.PersistentDataPath + "AssetBundles/" + GetPlatformName() + "/" + name;

            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            return FilePath.StreamingAssetsPath + "AssetBundles/" + GetPlatformName() + "/" + name;
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return "/AssetBundles/" + GetPlatformName() + "/"; }
        }

        #endregion

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
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

        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}