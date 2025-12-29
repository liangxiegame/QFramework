/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxie UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;

namespace QFramework
{
    [Serializable]
    public class PackageRepository
    {
        public string id;
        public string description;
        public string name;
        public string author;
        public string latestVersion;
        public DateTime createTime;
        public DateTime updateTime;
        public string latestDownloadUrl;
        public string installPath;
        public string[] includeFileOrFolders;
        public string accessRight;
        public string type;
        public bool isOfficial;
        public PackageVersionStatus status;
        

        internal int VersionNumber
        {
            get
            {
                var numbersStr = latestVersion.Replace("v", string.Empty).Split('.');

                var retNumber = numbersStr[2].ParseToInt();
                retNumber += numbersStr[1].ParseToInt() * 1000;
                retNumber += numbersStr[0].ParseToInt() * 1000000;
                return retNumber;
            }
        }
    }
}
#endif