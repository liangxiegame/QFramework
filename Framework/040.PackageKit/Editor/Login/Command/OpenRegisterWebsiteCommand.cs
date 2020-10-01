using UnityEngine;

namespace QFramework.PackageKit
{
    public class OpenRegisterWebsiteCommand : Command<PackageKitLoginApp>
    {
        public override void Execute()
        {
            Application.OpenURL("https://qframework.cn/user/register");
        }
    }
}