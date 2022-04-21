using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QFramework
{
    public class AssetBundleSettings
    {
        private static IResDatas mAssetBundleConfigFile = null;

        /// <summary>
        /// 默认
        /// </summary>
        private static Func<IResDatas> mAssetBundleConfigFileFactory = () => new ResDatas();

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

        public static List<IResDatas> SubProjectAssetBundleConfigFiles = new List<IResDatas>();
        
        public static bool LoadAssetResFromStreamingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadResFromStreamingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadResFromStreamingAssetsPath", value ? 1 : 0); }
        }


        #region AssetBundle 相关

        public static string AssetBundleUrl2Name(string url)
        {
            string retName = null;
            string parren = AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/";
            retName = url.Replace(parren, "");

            parren = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                     AssetBundlePathHelper.GetPlatformName() + "/";
            retName = retName.Replace(parren, "");
            return retName;
        }

        public static string AssetBundleName2Url(string name)
        {
            string retUrl = AssetBundlePathHelper.PersistentDataPath + "AssetBundles/" +
                            AssetBundlePathHelper.GetPlatformName() + "/" + name;

            if (File.Exists(retUrl))
            {
                return retUrl;
            }

            return AssetBundlePathHelper.StreamingAssetsPath + "AssetBundles/" +
                   AssetBundlePathHelper.GetPlatformName() + "/" + name;
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return "/AssetBundles/" + AssetBundlePathHelper.GetPlatformName() + "/"; }
        }

        #endregion
    }
}