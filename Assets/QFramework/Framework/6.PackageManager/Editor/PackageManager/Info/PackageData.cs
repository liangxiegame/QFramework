using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    [Serializable]
    public class PackageData
    {
        public string Name    = "";

        public string Version
        {
            get { return PackageVersions.First().Version; }
        }

        public string DownloadUrl
        {
            get { return PackageVersions.First().DownloadUrl; }
        }

        public Readme readme;
        
        public List<PackageVersion> PackageVersions = new List<PackageVersion>();

        public PackageData()
        {
            readme = new Readme();
        }
    }

    [Serializable]
    public class PackageVersion
    {
        public string Version;

        public string DownloadUrl;
    }
}