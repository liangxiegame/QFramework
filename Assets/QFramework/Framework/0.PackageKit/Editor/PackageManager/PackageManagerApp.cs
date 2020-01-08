namespace QFramework.PackageKit
{
    public class PackageManagerApp
    {
        public IQFrameworkContainer Container = new QFrameworkContainer();

        public PackageManagerApp()
        {
            // 注册好 自己的实例
            Container.RegisterInstance(Container);

            // 配置命令的执行
            TypeEventSystem.Register<IEditorStrangeMVCCommand>(OnCommandExecute);

            InstalledPackageVersions.Reload();

            // 注册好 model
            var model = new PackageManagerModel
            {
                Repositories = PackageInfosRequestCache.Get().PackageRepositories
            };

            Container.RegisterInstance(model);

            Container.Register<IPackageManagerServer, PackageManagerServer>();
        }

        void OnCommandExecute(IEditorStrangeMVCCommand command)
        {
            Container.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            TypeEventSystem.UnRegister<IEditorStrangeMVCCommand>(OnCommandExecute);

            Container.Clear();
            Container = null;
        }
    }
}