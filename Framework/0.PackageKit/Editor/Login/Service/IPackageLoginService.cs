using System;

namespace QFramework
{
    public interface IPackageLoginService
    {
        void DoGetToken(string username, string password, Action<string> onTokenGetted);
    }
}