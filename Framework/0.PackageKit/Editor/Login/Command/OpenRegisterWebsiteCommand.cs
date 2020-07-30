using UnityEngine;

namespace QFramework.PackageKit
{
    public class OpenRegisterWebsiteCommand : IPackageLoginCommand
    {
        public void Execute()
        {
            Application.OpenURL("https://qframework.cn/user/register");
        }
    }
}