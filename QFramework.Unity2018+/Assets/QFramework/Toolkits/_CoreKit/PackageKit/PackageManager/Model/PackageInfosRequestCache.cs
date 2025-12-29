#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public class PackageInfosRequestCache
    {
        public List<PackageRepository> PackageRepositories = new List<PackageRepository>();

        private static string mFilePath
        {
            get
            {
                var dirPath = Application.dataPath + "/.qframework/PackageManager/";

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                return dirPath + "PackageInfosRequestCache.json";
            }
        }

        internal static PackageInfosRequestCache Get()
        {
            if (File.Exists(mFilePath))
            {
                var cacheJson = File.ReadAllText(mFilePath);

                if (cacheJson.IsTrimNotNullAndEmpty())
                {
                    return new PackageInfosRequestCache();
                }
                try
                {
                    var retValue = JsonUtility.FromJson<PackageInfosRequestCache>(cacheJson);

                    if (retValue.PackageRepositories == null)
                    {
                        return new PackageInfosRequestCache();
                    }
                }
                catch (Exception)
                {
                    return new PackageInfosRequestCache();
                }
            }

            return new PackageInfosRequestCache();
        }

        internal void Save() => File.WriteAllText(mFilePath, JsonUtility.ToJson(this));
    }
}
#endif