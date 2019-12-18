namespace QFramework.PackageKit
{
    public class PackageLoginModel
    {
        public bool InLoginView = true;

        public bool Logined
        {
            get { return User.Logined; }
        }
    }
}