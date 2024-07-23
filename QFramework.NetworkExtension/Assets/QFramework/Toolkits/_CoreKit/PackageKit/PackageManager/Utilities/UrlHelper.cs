/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    internal class UrlHelper
    {
        public static string PackageUrl(PackageRepository p)
        {
            return "https://qframework.cn/package/detail/" + p.name;
        }
    }
}
#endif