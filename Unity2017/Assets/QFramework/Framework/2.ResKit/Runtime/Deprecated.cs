using System;
using UnityEngine;

namespace QFramework
{
    public class ResKitDeprecatedConfig
    {
        public const bool FORCE_DEPRECATED = true;
    }

    public partial class ResKit
    {
        [Obsolete("ResKit.ResDataFactory is depreacated,Please use AssetBundleSettings.AssetBundleConfigFileFactory instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static Func<IResDatas> ResDataFactory
        {
            set { AssetBundleSettings.AssetBundleConfigFileFactory = value; }
        }

        /// <summary>
        /// 获取自定义的 资源信息
        /// </summary>
        /// <returns></returns>
        [Obsolete("ResKit.ResData is depreacated,Please use AssetBundleSettings.AssetBundleConfigFile instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static IResDatas ResData
        {
            get
            {
                return AssetBundleSettings.AssetBundleConfigFile;
            }
            set { AssetBundleSettings.AssetBundleConfigFile = value; }
        }

        [Obsolete("ResKit.LoadResFromStreammingAssetsPath is depreacated,Please use AssetBundleSettings.LoadAssetResFromStreammingAssetsPath instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static bool LoadResFromStreammingAssetsPath
        {
            get { return AssetBundleSettings.LoadAssetResFromStreammingAssetsPath; }
            set { AssetBundleSettings.LoadAssetResFromStreammingAssetsPath = value; }
        }
    }
    
    public static class ResKitUtil
    {
        #region AssetBundle 相关

        
        [Obsolete("ResKitUtil.AssetBundleUrl2Name() is depreacated,Please use AssetBundleSettings.AssetBundleUrl2Name() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string AssetBundleUrl2Name(string url)
        {
            return AssetBundleSettings.AssetBundleUrl2Name(url);
        }

        [Obsolete("ResKitUtil.AssetBundleName2Url() is depreacated,Please use AssetBundleSettings.AssetBundleName2Url() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string AssetBundleName2Url(string name)
        {
            return AssetBundleSettings.AssetBundleName2Url(name);
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        [Obsolete("ResKitUtil.RELATIVE_AB_ROOT_FOLDER is depreacated,Please use AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER; }
        }
        
        #endregion
        
        [Obsolete("ResKitUtil.GetPlatformName() is depreacated,Please use AssetBundleSettings.GetPlatformName() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string GetPlatformName()
        {
            return AssetBundleSettings.GetPlatformName();
        }
	
#if UNITY_EDITOR
        [Obsolete("ResKitUtil.GetPlatformForAssetBundles() is depreacated,Please use AssetBundleSettings.GetPlatformForAssetBundles() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string GetPlatformForAssetBundles(UnityEditor.BuildTarget target)
        {
            return AssetBundleSettings.GetPlatformForAssetBundles(target);
        }
#endif
        
        [Obsolete("ResKitUtil.GetPlatformForAssetBundles() is depreacated,Please use AssetBundleSettings.GetPlatformForAssetBundles() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            return AssetBundleSettings.GetPlatformForAssetBundles(platform);
        }
    }
}