using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit.Model
{
    public interface IInstalledPackageVersionsConfigModel : IModel
    {
        List<PackageVersion> PackageVersions { get; }

        void Reload();

        PackageVersion GetByName(string name);
    }

    public class InstalledPackageVersionsConfigModel : Model<PackageKit>, IInstalledPackageVersionsConfigModel
    {


        public List<PackageVersion> PackageVersions { get; private set; }

        public void Reload()
        {
            PackageVersions.Clear();

            var versionFiles = Array.FindAll(AssetDatabase.GetAllAssetPaths(),
                name => name.EndsWith("PackageVersion.json"));

            foreach (var fileName in versionFiles)
            {
                var text = File.ReadAllText(fileName);
                PackageVersions.Add(JsonUtility.FromJson<PackageVersion>(text));
            }
        }

        public PackageVersion GetByName(string name)
        {
            return PackageVersions.Find(packageVersion => packageVersion.Name == name);
        }

        public InstalledPackageVersionsConfigModel()
        {
            PackageVersions = new List<PackageVersion>();
            
            Reload();
        }
    }
}