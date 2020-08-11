namespace QFramework.PackageKit.State
{
    public class PackageKitLoginState
    {
        public static Property<bool> InLoginView = new Property<bool>(false);

        public static CustomProperty<bool> Logined =
            new CustomProperty<bool>(() => User.Logined, value =>
            {
                if (!value)
                {
                    User.Clear();
                }
            });

        public static CustomProperty<bool> LoginViewVisible = new CustomProperty<bool>(
            () => !User.Logined && mInLoginView, (value) => mInLoginView = value
        );

        private static bool mInLoginView = true;

        public static Property<bool> RegisterViewVisible = new CustomProperty<bool>(
            () => !User.Logined && !mInLoginView,
            (value) => mInLoginView = !value);
    }
}