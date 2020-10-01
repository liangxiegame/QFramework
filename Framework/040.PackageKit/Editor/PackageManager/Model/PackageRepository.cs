using System;
using System.IO;

namespace QFramework.PackageKit
{
    [Serializable]
    public class PackageRepository
    {
        public string   id;
        public string   description;
        public string   name;
        public string   author;
        public string   latestVersion;
        public DateTime createTime;
        public DateTime updateTime;
        public string   latestDownloadUrl;
        public string   installPath;
        public string   accessRight;
        public string   type;

        public int VersionNumber
        {
            get
            {
                var numbersStr = latestVersion.Replace("v", string.Empty).Split('.');

                var retNumber = numbersStr[2].ParseToInt();
                retNumber += numbersStr[1].ParseToInt() * 100;
                retNumber += numbersStr[0].ParseToInt() * 10000;
                return retNumber;
            }

        }

        public bool Installed
        {
            get { return Directory.Exists(installPath); }
        }
    }
}