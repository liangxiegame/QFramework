using System;

namespace QFramework.PackageKit
{
    public interface IPackageLoginService
    {
        void DoGetToken(string username, string password, Action<string> onTokenGetted);
    }
}