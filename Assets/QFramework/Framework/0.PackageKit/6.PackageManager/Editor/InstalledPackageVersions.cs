using System;
using System.Collections.Generic;
using QF.Extensions;
using UnityEditor;

namespace QF
{
    public class InstalledPackageVersions
    {
        private static List<PackageVersion> mPackageVersions = new List<PackageVersion>();

        public static List<PackageVersion> Get()
        {
            if (mPackageVersions.Count == 0)
            {
                Reload();
            }

            return mPackageVersions;
        }

        public static void Reload()
        {
            mPackageVersions.Clear();

            var versionFiles = Array.FindAll(AssetDatabase.GetAllAssetPaths(),
                name => name.EndsWith("PackageVersion.json"));

            versionFiles.ForEach(fileName =>
            {
                mPackageVersions.Add(SerializeHelper.LoadJson<PackageVersion>(fileName));
            });
        }

        public static PackageVersion FindVersionByName(string name)
        {
            return Get().Find(installedPackageVersion => installedPackageVersion.Name == name);
        }
    }
}