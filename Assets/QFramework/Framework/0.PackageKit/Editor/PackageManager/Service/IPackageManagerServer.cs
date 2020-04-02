using System;
using System.Collections.Generic;

namespace QFramework.PackageKit
{
    public interface IPackageManagerServer
    {

        void DeletePackage(string packageId, Action onResponse);


        void GetAllRemotePackageInfoV5(Action<List<PackageRepository>, List<string>> onResponse);
    }
}