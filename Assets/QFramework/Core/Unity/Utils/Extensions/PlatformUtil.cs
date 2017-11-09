/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    using UnityEngine;

    /// <summary>
    /// 平台相关的应该全部扔到这里
    /// </summary>
    public class PlatformUtil
    {
        public const string ABundlesOutputForderName = "QAB";

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(Application.platform);
        }

        static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}