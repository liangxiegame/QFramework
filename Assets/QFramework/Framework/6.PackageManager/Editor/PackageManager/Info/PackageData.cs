using System;

namespace QFramework
{
    [Serializable]
    public class PackageData
    {
        public string name    = "";
        public string version = "";
        public string type    = "";
        public Readme readme;
        public string url         = "";

        public PackageData()
        {
            readme = new Readme();
        }
    }
}