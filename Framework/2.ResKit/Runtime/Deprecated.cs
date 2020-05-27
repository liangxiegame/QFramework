using System;
using UnityEngine;

namespace QFramework
{
    public class ResKitDeprecatedConfig
    {
        public const bool FORCE_DEPRECATED = false;
    }
    
    public static class ResKitUtil
    {
        #region AssetBundle 相关

        
        [Obsolete("ResKitUtil.AssetBundleUrl2Name() is depreacated,Please use AssetBundleUtil.AssetBundleUrl2Name() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string AssetBundleUrl2Name(string url)
        {
            return AssetBundleUtil.AssetBundleUrl2Name(url);
        }

        [Obsolete("ResKitUtil.AssetBundleName2Url() is depreacated,Please use AssetBundleUtil.AssetBundleName2Url() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string AssetBundleName2Url(string name)
        {
            return AssetBundleUtil.AssetBundleName2Url(name);
        }

        //导出目录

        /// <summary>
        /// AssetBundle存放路径
        /// </summary>
        [Obsolete("ResKitUtil.RELATIVE_AB_ROOT_FOLDER is depreacated,Please use AssetBundleUtil.RELATIVE_AB_ROOT_FOLDER instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return AssetBundleUtil.RELATIVE_AB_ROOT_FOLDER; }
        }
        
        #endregion
        
        [Obsolete("ResKitUtil.GetPlatformName() is depreacated,Please use AssetBundleUtil.GetPlatformName() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string GetPlatformName()
        {
            return AssetBundleUtil.GetPlatformName();
        }
	
#if UNITY_EDITOR
        [Obsolete("ResKitUtil.GetPlatformForAssetBundles() is depreacated,Please use AssetBundleUtil.GetPlatformForAssetBundles() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        public static string GetPlatformForAssetBundles(UnityEditor.BuildTarget target)
        {
            return AssetBundleUtil.GetPlatformForAssetBundles(target);
        }
#endif
        
        [Obsolete("ResKitUtil.GetPlatformForAssetBundles() is depreacated,Please use AssetBundleUtil.GetPlatformForAssetBundles() instead", ResKitDeprecatedConfig.FORCE_DEPRECATED)]
        static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            return AssetBundleUtil.GetPlatformForAssetBundles(platform);
        }
    }
}