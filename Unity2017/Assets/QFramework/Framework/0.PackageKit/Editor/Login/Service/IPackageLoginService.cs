using System;

namespace QFramework
{
    public interface IPackageLoginService: IModel
    {
        void DoGetToken(string username, string password, Action<string> onTokenGetted);
    }
}