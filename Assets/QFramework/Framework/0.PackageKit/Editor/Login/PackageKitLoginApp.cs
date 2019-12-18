namespace QFramework.PackageKit
{
    public class PackageLoginApp
    {
        private static IQFrameworkContainer mContainer { get; set; }

        public static IQFrameworkContainer Container
        {
            get { return mContainer; }
        }
        
        public PackageLoginApp()
        {
            mContainer = new QFrameworkContainer();

            mContainer.RegisterInstance(new PackageLoginModel());
            mContainer.RegisterInstance<IPackageLoginService>(new PacakgeLoginService());
            TypeEventSystem.Register<IPackageLoginCommand>(OnCommandExecute);
        }

        void OnCommandExecute(IPackageLoginCommand command)
        {
            mContainer.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            TypeEventSystem.UnRegister<IPackageLoginCommand>(OnCommandExecute);

            mContainer.Clear();
            mContainer = null;
        }
    }
}